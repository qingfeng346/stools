import database from "../database.js"
class AlbumManager {
    constructor() {
        this.pendingIds = []
    }
    async GetInfoByName(name) {
        return (await database.album.findOrCreate({ where: { name: name } }))[0].dataValues
    }
    async UpdateAlbumInfo(id, artist) {
        await database.album.update({ artist: artist }, { where: { id: id } })
    }
}
export default new AlbumManager()