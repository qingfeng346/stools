const net = require('./net')
const database = require('./database')
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
            // where: {
            //     id: {
            //         [Op.like]: `%${searchValue}%`
            //     }
            // }
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
    OnMusicDownload(data) {

    }
}
module.exports = new music()