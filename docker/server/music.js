const net = require('./net')
const database = require('./database')
const { Util, FileUtil, logger } = require('weimingcommons')
class music {
    async init() {
        Util.Encoding = "utf8"
        net.register("musiclist", this.OnMusicList.bind(this))
        net.register("musicdownload", this.OnMusicDownload.bind(this))
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
        let file = Util.getTempFile(".json")
        let dir = `${process.cwd()}/stools`
        logger.info("工作目录: " + dir)
        if (msg.type == "music") {
            await Util.execAsync("dotnet", dir, [ "run", "downloadmusic", "-url", msg.url, "-output", `${process.cwd()}/data/music`, "-path", 3, "-exportFile", file], { shell: true} )
        } else if (msg.type == "album") {
            await Util.execAsync("dotnet", dir, [ "run", "downloadalbum", "-url", msg.url, "-output", `${process.cwd()}/data/music`, "-path", 3, "-exportFile", file], { shell: true})
        }
        let infos = await FileUtil.GetFileJsonAsync(file)
        if (infos == null) return
        for (let info of infos) {
            let data = {
                musicId: info.id,
                musicType: info.type,
                name: info.name,
                album: info.album,
                singer: info.singer,
                year: info.year,
                size: info.size,
                path: info.path,
                duration: info.duration,
                time: new Date().valueOf()
            }
            logger.info(`下载音乐成功 : ${JSON.stringify(data)}`)
            await database.music.upsert(data, { 
                where: { musicId: info.id, musicType: info.type } 
            })
        }
    }
}
module.exports = new music()