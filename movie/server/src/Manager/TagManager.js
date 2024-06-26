const database = require("../database")
class TagManager {
    async GetTagId(name) {
        let value = (await database.tag.findOrCreate({ where: { name: name } }))[0].dataValues
        return value.id
    }
}
module.exports = new TagManager()