const { Util } = require("weimingcommons")
const database = require("../database")
class ImageManager {
    async GetImageInfoByUrl(url) {
        if (Util.isNullOrEmpty(url))
            return null
        return (await database.image.findOrCreate({ where: { url: url } }))[0].dataValues
    }
}
module.exports = new ImageManager()