const utils = require('../utils');
const cheerio = require('cheerio');
class ProviderTest3 {
    async SearchResult(name) {
        let result = await utils.get(`http://avsox.click/ja/search/${name}`)
        let $ = cheerio.load(result.data)
        let first = $($("div[class='item']").first())
        return first.find("a").attr("href")
    }
    async GetMovieInfo(name) {
        let searchResult = await this.SearchResult(name)
        var url = `http:${searchResult}`;
        let result = await utils.get(url)
        if (result == null) return
        let $ = cheerio.load(result.data)
        let title = $("h3").text()
        let imageUrl = $(`a[class='bigImage']`).attr("href")
        let movieInfo = {
            movieId : name,
            title: title,
            imageUrl : imageUrl,
            thumbUrl : imageUrl,
            actors: [],
            tags: [],
            makers: [],
            labels: [],
            series: [],
            shotscreens: []
        }
        let castNodes = $("a[class='avatar-box']")
        castNodes.each(function(index, element) {
            movieInfo.actors.push($(element).find("span").text())
        })
        let genreNodes = $("span[class='genre']")
        genreNodes.each(function(index, element) {
            movieInfo.tags.push($(element).find("a").text())
        })
        return movieInfo
    }
    async GetPersonInfo(name) {
    }
}
module.exports = new ProviderTest3()