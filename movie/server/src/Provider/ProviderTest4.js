const utils = require('../utils');
const cheerio = require('cheerio');
class ProviderTest4 {
    async GetMovieInfo(name) { }
    async ParseMovieInfo(name, content) { }
    async GetPersonInfo(name) { }
    async ParsePersonInfo(name, content) {
        if (content == null) return;
        let $ = cheerio.load(content)
        let personInfo = {}
        personInfo.name = name
        personInfo.imageUrl = $($("meta[property='og:image']").first()).attr("content")
        personInfo.desc = $($("p[itemprop='description']").first()).text()
        personInfo.info = {}
        let infoNodes = $($("div[class='player-col']").first()).find("li")
        infoNodes.each(function(index, element) {
            let key = $($(element).find("label").first()).text()
            let value = $($(element).find("span").first()).text()
            personInfo.info[key] = value
        })
        return personInfo
    }
}
module.exports = new ProviderTest4()