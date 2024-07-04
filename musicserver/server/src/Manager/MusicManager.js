const { FileUtil, logger, Util } = require("weimingcommons")
const path = require('path')
const fs = require('fs')
const database = require("../database")
const ActorManager = require("./ActorManager")
const { QueryTypes } = require("sequelize")
const ProviderManager = require("../Provider/ProviderManager")
const { AssetsPath } = require("../config")
const ImageManager = require("./ImageManager")
const utils = require("../utils")
const MP3Tag = require('mp3tag.js')
class MusicManager {
    constructor() {
        this.pendingIds = []
        this.musicRoot = path.resolve(AssetsPath, "music")
    }
    async UpdateMovieList() {
        let allDatas = await database.music.findAll()
        let allFiles = FileUtil.GetFiles(this.musicRoot, true)
        if (allFiles == null) return
        let files = []
        for (let file of allFiles) {
            if (file.endsWith(".mp3")) {
                let f = file.substring(this.musicRoot.length + 1)
                files.push(f)
                await this.GetMusicInfoByPath(f)
            }
        }
        for (let data of allDatas) {
            if (files.indexOf(data.path) < 0) {
                logger.info(`文件 : ${data.path} 已删除 : ${data.id}`)
                await database.music.destroy({ where: {id : data.id}})
            }
        }
    }
    async GetMovieList(type, value, page, pageSize) {
        let values = undefined
        value = decodeURI(value)
        if (type == null || type == "all") {
            values = await database.music.findAll({attributes: ["id", "movieId", "title", "path", "thumbUrl", "isInfo"]})
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
    async GetMusicInfoByPath(path) {
        let value = (await database.music.findOrCreate({ where: { path: path } }))[0].dataValues
        this.CheckRefreshInfo(value)
        return value
    }
    async GetMovieInfoById(id) {
        let value = await database.music.findOne({ where: { id: id } })
        this.CheckRefreshInfo(value)
        return value
    }
    UpdateMusicInfo(id) {
        if (this.pendingIds.indexOf(id) < 0) {
            this.pendingIds.push(id)
        }
    }
    ParseMovieInfo(id, type, content) {
        this.pendingParse.push({id: id, type: type, content: content})
    }
    CheckRefreshInfo(value) {
        if (!value?.isInfo) {
            this.UpdateMusicInfo(value.id)
        }
    }
    async update() {
        if (this.pendingIds.length > 0) { 
            await this.RefreshInfo(this.pendingIds.pop())
        }
    }
    async RefreshInfo(id) {
        let value = await database.music.findOne({ where: { id: id } })
        if (value == null) {
            throw new Error(`找不到MovieId:${id}`)
        }
        let mp3tag = new MP3Tag(fs.readFileSync(`${this.musicRoot}/${value.path}`))
        mp3tag.read()
        console.log(mp3tag.tags)
        let musicInfo = {
            title: mp3tag.tags.title,
            year: mp3tag.tags.v2.TYER,
            track: mp3tag.tags.v2.TRCK,
            // lyrics: mp3tag.tags.v2.USLT[0].text,
            isInfo: true,
        }
        FileUtil.CreateFile(`${AssetsPath}/cache/music/${id}.jpg`, Buffer.from(mp3tag.tags.v2.APIC[0].data))
        await database.music.update(musicInfo, { where: {id: id}})
    }
}
module.exports = new MusicManager()