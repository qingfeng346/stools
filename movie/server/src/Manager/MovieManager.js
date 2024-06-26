const { FileUtil, logger, Util } = require("weimingcommons")
const path = require('path')
const database = require("../database")
const ActorManager = require("./ActorManager")
const { QueryTypes } = require("sequelize")
const ProviderManager = require("../Provider/ProviderManager")
const { AssetsPath } = require("../config")
class MovieManager {
    constructor() {
        this.pendingIds = []
        this.mediaRoot = path.resolve(AssetsPath, "media")
    }
    async UpdateMovieList() {
        let movies = await database.movie.findAll()
        let allFiles = FileUtil.GetFiles(this.mediaRoot, true)
        if (allFiles == null) return
        let files = []
        for (let file of allFiles) {
            if (file.endsWith(".mp4") ||
                file.endsWith(".mkv") ||
                file.endsWith(".avi")) {
                let f = file.substring(this.mediaRoot.length + 1)
                files.push(f)
                await Util.sleep(1000)
                await this.GetMovieInfoByPath(f)
            }
        }
        for (let movie of movies) {
            if (files.indexOf(movie.path) < 0) {
                logger.info(`文件 : ${movie.path} 已删除`)
                await database.movie.destroy({ where: {id : movie.id}})
            }
        }
    }
    async GetMovieInfoByPath(path) {
        let value = (await database.movie.findOrCreate({ where: { path: path } }))[0].dataValues
        if (!value.isInfo) {
            logger.info(`添加新文件:${path}`)
            if (this.pendingIds.indexOf(value.id) < 0) {
                this.pendingIds.push(value.id)
            }
        }
        return value
    }
    async GetMovieInfoById(id) {
        let value = await database.movie.findOne({ where: { id: id } })
        if (!value.isInfo) {
            if (this.pendingIds.indexOf(value.id) < 0) {
                this.pendingIds.push(value.id)
            }
        }
        return value
    }
    async update() {
        while (this.pendingIds.length > 0) {
            await this.RefreshInfo(this.pendingIds.pop())
        }
    }
    async RefreshInfo(id) {
        let value = await database.movie.findOne({ where: { id: id } })
        if (value == null) {
            throw new Error(`找不到MovieId:${id}`)
        }
        let movieInfo = await ProviderManager.GetMovieInfo(FileUtil.GetFileNameWithoutExtension(value.path))
        value = value.dataValues
        value.isInfo = true
        value.title = movieInfo.title
        value.desc = movieInfo.desc
        value.thumbUrl = movieInfo.thumbUrl
        value.imageUrl = movieInfo.imageUrl
        if (movieInfo.actors.length > 0) {
            value.actors = []
            for (let v of movieInfo.actors) {
                value.actors.push((await ActorManager.GetActorInfoByName(v)).id)
            }
        }
        if (movieInfo.tags.length > 0) {
            value.tags = []
            for (let v of movieInfo.tags) {
                value.tags.push(v)
            }
        }
        if (movieInfo.shotscreens.length > 0) {
            value.shotscreens = []
            for (let v of movieInfo.shotscreens) {
                value.shotscreens.push(v)
            }
        }
        await database.movie.update(value, { where: {id: id}})
    }
    async GetAllMovieInfos() {
        await database.movie.findAll()
    }
    async GetAllMovieInfosByActor(id) {
        return await database.sequelize.query(`select * from \`movie\` where exists (select 1 from json_each(actors) where value = ${id})`, { type: QueryTypes.SELECT })
    }
    async GetAllMovieInfosByTag(id) {
        return await database.sequelize.query(`select * from \`movie\` where exists (select 1 from json_each(tags) where value = ${id})`, { type: QueryTypes.SELECT })
    }
}
module.exports = new MovieManager()