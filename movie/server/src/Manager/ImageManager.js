const { Util } = require("weimingcommons")
const database = require("../database")
const utils = require("../utils")
const { AssetsPath } = require("../config")
class ImageManager {
    constructor() {
        this.pendingIds = []
    }
    async GetImageInfoByUrl(url) {
        if (Util.isNullOrEmpty(url))
            return null
        var value = (await database.image.findOrCreate({ where: { url: url } }))[0].dataValues
        this.CheckRefreshInfo(value)
        return value
    }
    CheckRefreshInfo(value) {
        if (!value?.isInfo) {
            if (this.pendingIds.indexOf(value.id) < 0) {
                this.pendingIds.push(value.id)
            }
        }
    }
    async update() {
        if (this.pendingIds.length > 0) {
            await this.RefreshInfo(this.pendingIds.pop())
        }
    }
    async RefreshInfo(id) {
        let value = await database.image.findOne({ where: { id: id } })
        if (value == null) {
            throw new Error(`找不到ImageId:${id}`)
        }
        value = value.dataValues
        let error = await utils.DownloadFile(value.url, `${AssetsPath}/cache/images/${id}.png`)
        if (error == null) {
            value.isInfo = true
            await database.image.update(value, { where: {id: id}})
        }
    }
}
module.exports = new ImageManager()