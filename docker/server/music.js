const net = require('./net')
const database = require('./database')
const { Util } = require('weimingcommons')
class music {
    async init() {
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
    async OnMusicDownload(data) {
        if (data.type == "music") {
            await Util.execAsync("stools.exe", process.cwd(), [ "downloadmusic", "-url", data.url, "-output", "data", "-path", 3, "-exportFile", "aaa.json"])
        }
    }
}
module.exports = new music()