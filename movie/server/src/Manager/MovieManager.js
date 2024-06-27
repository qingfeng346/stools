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
                await Util.sleep(100)
                await this.GetMovieInfoByPath(f)
            }
        }
        for (let movie of movies) {
            if (files.indexOf(movie.path) < 0) {
                logger.info(`文件 : ${movie.path} 已删除 : ${movie.id}`)
                await database.movie.destroy({ where: {id : movie.id}})
            }
        }
    }
    async GetMovieList(type, value, page, pageSize) {
        let values = undefined
        if (type == null || type == "all") {
            values = await database.movie.findAll({attributes: ["id", "title", "path", "thumbUrl", "isInfo"]})
        } else {
            values = await database.sequelize.query(`select id,title,path,thumbUrl,isInfo from \`movie\` where exists (select 1 from json_each(${type}s) where value = '${value}')`, { type: QueryTypes.SELECT })
        }
        if (values == null) {
            values = []
        }
        for (let value of values) {
            this.CheckRefreshInfo(value)
        }
        return values
    }
    async GetMovieInfoByPath(path) {
        let value = (await database.movie.findOrCreate({ where: { path: path } }))[0].dataValues
        this.CheckRefreshInfo(value)
        return value
    }
    async GetMovieInfoById(id) {
        let value = await database.movie.findOne({ where: { id: id } })
        this.CheckRefreshInfo(value)
        return value
    }
    UpdateMoveInfo(id) {
        if (this.pendingIds.indexOf(id) < 0) {
            this.pendingIds.push(id)
        }
    }
    CheckRefreshInfo(value) {
        if (!value?.isInfo) {
            this.UpdateMoveInfo(value.id)
        }
    }
    async update() {
        if (this.pendingIds.length > 0) { 
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
        if (movieInfo != null) {
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
        } else {
            value.title = "title"
            value.desc = "desc"
            value.actors = []
            value.actors.push((await ActorManager.GetActorInfoByName("actor1")).id)
            value.tags = ["tag1"]
            value.makers = ["maker1"]
            value.genres = ["genre1"]
            value.series = ["serie1"]
        }
        await database.movie.update(value, { where: {id: id}})
    }
}
module.exports = new MovieManager()