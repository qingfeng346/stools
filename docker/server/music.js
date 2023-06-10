const net = require('./net')
const database = require('./database')
const { Util, FileUtil, logger } = require('weimingcommons')
class music {
    async init() {
        this.list = []
        Util.Encoding = "utf8"
        net.register("musiclist", this.OnMusicList.bind(this))
        net.register("musicdownload", this.OnMusicDownload.bind(this))
        net.register("musicdelete", this.OnMusicDelete.bind(this))
        this.CheckDownload()
    }
    async OnMusicList(data) {
        let pageSize = data.pageSize
        let page = data.page
        let condition = {
            limit: pageSize,
            offset: (page - 1) * pageSize,
            order: [["time", "DESC"]],
        }
        let total = await database.music.count()
        let datas = await database.music.findAll(condition)
        let result = {}
        result.total = total
        result.pageSize = pageSize
        result.page = page
        result.datas = datas
        return result
    }
    async OnMusicDownload(msg) {
        let index = msg.url.indexOf("&")
        if (index > 0) {
            msg.url = msg.url.substring(0, index)
        }
        this.list.splice(0, 0, msg)
    }
    async CheckDownload() {
        while (true) {
            while (this.list.length > 0) {
                let msg = this.list.pop()
                logger.info(`开始下载:${JSON.stringify(msg)}  剩余数量:${this.list.length}`)
                await this.Download(msg)
            }
            await Util.sleep(3000)
        }
    }
    async Download(msg) {
        let file = Util.getTempFile(".json")
        let dir = `${process.cwd()}/stools`
        if (msg.type == "music") {
            await Util.execAsync("dotnet", dir, [ "run", "downloadmusic", "-output", `${process.cwd()}/music`, "-path", 1, "-exportFile", file, "-url", msg.url], { shell: true} )
        } else if (msg.type == "album") {
            await Util.execAsync("dotnet", dir, [ "run", "downloadalbum", "-output", `${process.cwd()}/music`, "-path", 1, "-exportFile", file, "-url", msg.url], { shell: true})
        }
        let infos = await FileUtil.GetFileJsonAsync(file)
        if (infos == null) return
        for (let info of infos) {
            let data = {
                path: info.path,
                musicId: info.id,
                musicType: info.type,
                name: info.name,
                album: info.album,
                singer: info.singer,
                year: info.year,
                size: info.size,
                duration: info.duration,
                time: new Date().valueOf()
            }
            await database.music.upsert(data, { where: { path: info.path } })
            logger.info(`下载音乐成功 : ${JSON.stringify(data)}`)
            net.sendMessage("downloadSuccess", data)
        }
    }
    async OnMusicDelete(msg) {
        await FileUtil.DeleteFileAsync(msg.path)
        await database.music.destroy({ where: { path: msg.path } })
        logger.notify(`删除《${msg.path}》成功`)
    }
}
module.exports = new music()