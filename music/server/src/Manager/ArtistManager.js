import database from "../database.js"
class ArtistManager {
    constructor() {
        this.pendingIds = []
    }
    async GetInfoByName(name) {
        return (await database.artist.findOrCreate({ where: { name: name } }))[0].dataValues
    }
}
export default new ArtistManager()