import database from "../database.js"
class AlbumManager {
    constructor() {
        this.pendingIds = []
    }
    async GetInfoByName(name) {
        return (await database.album.findOrCreate({ where: { name: name } }))[0].dataValues
    }
}
export default new AlbumManager()