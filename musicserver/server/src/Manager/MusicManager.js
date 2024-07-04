import { FileUtil, logger } from "weimingcommons"
import path from 'path'
import fs from 'fs'
import database from "../database.js"
import { QueryTypes } from "sequelize"
import { AssetsPath } from "../config.js"
import { parseStream } from "music-metadata"
class MusicManager {
    constructor() {
        this.pendingIds = []
        this.requestRefreshList = true
        this.musicRoot = path.resolve(AssetsPath, "music")
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
    async update() {
        if (this.requestRefreshList) {
            await this.RefreshMusicList()
        }
        if (this.pendingIds.length > 0) { 
            await this.RefreshInfo(this.pendingIds.pop())
        }
    }
    async RefreshMusicList() {
        let allFiles = FileUtil.GetFiles(this.musicRoot, true)
        if (allFiles == null) return
        let existIds = new Set()
        for (let file of allFiles) {
            if (file.endsWith(".mp3")) {
                let value = (await database.music.findOrCreate({ where: { path: file.substring(this.musicRoot.length + 1) } }))[0].dataValues
                let fileStat = fs.statSync(file)
                if (value.size != fileStat.size || value.ctime != fileStat.ctime) {
                    this.UpdateMusicInfo(value.id)
                }
                existIds.add(value.id)
            }
        }
        let allDatas = await database.music.findAll({attributes: ["id", "path"]})
        for (let data of allDatas) {
            if (!existIds.has(data.id)) {
                logger.info(`文件 : ${data.path} 已删除 : ${data.id}`)
                await database.music.destroy({ where: {id : data.id}})
            }
        }
    }
    async RefreshInfo(id) {
        let value = await database.music.findOne({ where: { id: id } })
        if (value == null) {
            throw new Error(`找不到MovieId:${id}`)
        }
        let filePath = `${this.musicRoot}/${value.path}`
        let fileStat = fs.statSync(filePath)
        let metadata = await parseStream(fs.createReadStream(filePath))
        let musicInfo = {
            size: fileStat.size,
            ctime: fileStat.ctime,
            version: value.version != null ? value.version + 1 : 1,
            title: metadata.common.title,
            year: metadata.common.year,
            track: metadata.common.track?.no,
        }
        if (metadata.common.lyrics != null && metadata.common.lyrics.length() > 0) {
            musicInfo.lyrics = metadata.common.lyrics[0]
        }
        if (metadata.common.picture != null && metadata.common.picture.length() > 0) {
            FileUtil.CreateFile(`${AssetsPath}/cache/music/${id}.jpg`, Buffer.from(metadata.common.picture[0].data))
        }
        await database.music.update(musicInfo, { where: {id: id}})
    }
}
export default new MusicManager()