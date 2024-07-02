const { FileUtil, logger, Util } = require("weimingcommons")
const path = require('path')
const database = require("../database")
const ActorManager = require("./ActorManager")
const { QueryTypes } = require("sequelize")
const ProviderManager = require("../Provider/ProviderManager")
const { AssetsPath } = require("../config")
const ImageManager = require("./ImageManager")
const utils = require("../utils")
class MovieManager {
    constructor() {
        this.pendingIds = []
        this.pendingParse = []
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
        value = decodeURI(value)
        if (type == null || type == "all") {
            values = await database.movie.findAll({attributes: ["id", "movieId", "title", "path", "thumbUrl", "isInfo"]})
        } else if (type == "actor") {
            values = await database.sequelize.query(`select id,movieId,title,path,thumbUrl,isInfo from \`movie\` where exists (select 1 from json_each(${type}s) where value = ${value})`, { type: QueryTypes.SELECT })
        } else {
            values = await database.sequelize.query(`select id,movieId,title,path,thumbUrl,isInfo from \`movie\` where exists (select 1 from json_each(${type}s) where value = '${value}')`, { type: QueryTypes.SELECT })
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
    ParseMovieInfo(id, type, content) {
        this.pendingParse.push({id: id, type: type, content: content})
    }
    CheckRefreshInfo(value) {
        if (!value?.isInfo) {
            this.UpdateMoveInfo(value.id)
        }
    }
    async update() {
        if (this.pendingIds.length > 0) { 
            await this.RefreshInfo(this.pendingIds.pop())
        } else if (this.pendingParse.length > 0) {
            let data = this.pendingParse.pop()
            await this.RefreshInfo(data.id, data.type, data.content)
        }
    }
    async RefreshInfo(id, type, content) {
        let value = await database.movie.findOne({ where: { id: id } })
        if (value == null) {
            throw new Error(`找不到MovieId:${id}`)
        }
        let movieInfo = null
        if (type == null || content == null) {
            movieInfo = await ProviderManager.GetMovieInfo(FileUtil.GetFileNameWithoutExtension(value.path))
        } else {
            movieInfo = await ProviderManager.ParseMovieInfo(FileUtil.GetFileNameWithoutExtension(value.path), type, content)
        }
        value = value.dataValues
        value.isInfo = true
        value.thumbUrl = (await ImageManager.GetImageInfoByUrl(movieInfo?.thumbUrl ?? utils.getDefaultImage(150,200)))?.id
        value.imageUrl = (await ImageManager.GetImageInfoByUrl(movieInfo?.imageUrl ?? utils.getDefaultImage(400,250)))?.id
        if (movieInfo != null) {
            value.movieId = movieInfo.movieId
            value.title = movieInfo.title
            value.desc = movieInfo.desc
            value.releaseDate = movieInfo.releaseDate
            if (movieInfo.actors.length > 0) {
                value.actors = []
                for (let v of movieInfo.actors) {
                    value.actors.push((await ActorManager.GetActorInfoByName(v)).id)
                }
            }
            if (movieInfo.tags.length > 0) {
                value.tags = movieInfo.tags
            }
            if (movieInfo.makers.length > 0) {
                value.makers = movieInfo.makers
            }
            if (movieInfo.labels.length > 0) {
                value.labels = movieInfo.labels
            }
            if (movieInfo.series.length > 0) {
                value.series = movieInfo.series
            }
            if (movieInfo.shotscreens.length > 0) {
                value.shotscreens = movieInfo.shotscreens
            }
        }
        await database.movie.update(value, { where: {id: id}})
    }
}
module.exports = new MovieManager()