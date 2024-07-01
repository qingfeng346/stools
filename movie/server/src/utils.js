const axios = require('axios')
const { logger, FileUtil } = require('weimingcommons')
class utils {
    async get(url) {
        return await axios.get(url).then(function(res) {
            return res
        }).catch(function(error) {
            logger.error(`GetUrl ${url} is error : ${error.message}\n${error.stack}`)
        })
    }
    getParam(url, key) {
        let params = url.substring(url.lastIndexOf("?") + 1).split("&")
        for (let str of params) {
            let index = str.indexOf("=")
            if (index <= 0) { continue }
            let name = str.substring(0, index);
            if (name == key) {
                return str.substring(index + 1)
            }
        }
        return ""
    }
    getDefaultImage(width, height) {
        return `https://fakeimg.pl/${width}x${height}?text=No%20Image`
    }
    DownloadFile(fileUrl, filePath) {
        FileUtil.CreateDirectoryByFile(filePath)
        return new Promise((resolve, reject) => {
            axios({
                method: 'get',
                url: fileUrl,
                responseType: 'stream'
            })
            .then(response => {
                response.data.pipe(fs.createWriteStream(filePath));
            })
            .then(() => {
                resolve()
            })
            .catch(error => {
                console.error(`Error downloading file: ${fileUrl}`, error);
                resolve(error)
            });
        })
    }
}
module.exports = new utils()