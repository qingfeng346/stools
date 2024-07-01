const axios = require('axios')
const { logger } = require('weimingcommons')
class utils {
    async get(url) {
        return await axios.get(url).then(function(res) {
            return res
        }).catch(function(error) {
            logger.error(`GetUrl ${url} is error : ${error.message}\n${error.stack}`)
        })
    }
    getDefaultImage(width, height) {
        return `https://fakeimg.pl/${width}x${height}?text=No%20Image`
    }
}
module.exports = new utils()