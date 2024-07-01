const utils = require('../utils');
const cheerio = require('cheerio');
class ProviderTest2 {
    async GetMovieInfo(name) {
        var url = `https://jav5.land/ja/id_search.php?keys=${name}`;
        return await this.ParseMovieInfo(name, (await utils.get(url)).data)
    }
    async ParseMovieInfo(name, content) {
        if (content == null) return
        let $ = cheerio.load(content)
        let title = $("span[class='glyphicon glyphicon-film']").parent().text()
        let imageUrl = $(`img[alt='${name}']`).attr("src")
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
        let castNodes = $("span[class='cast']")
        castNodes.each(function(index, element) {
            movieInfo.actors.push($(element).find("a").text())
        })
        let genreNodes = $("span[class='genre']")
        genreNodes.each(function(index, element) {
            movieInfo.tags.push($(element).find("a").text())
        })
        let makerNodes = $("span[class='maker']")
        makerNodes.each(function(index, element) {
            movieInfo.makers.push($(element).find("a").text())
        })
        let serieNodes = $("span[class='series']")
        serieNodes.each(function(index, element) {
            movieInfo.series.push($(element).find("a").text())
        })
        let labelNodes = $("span[class='label1']")
        labelNodes.each(function(index, element) {
            movieInfo.labels.push($(element).find("a").text())
        })
        let shotscreensNodes = $("span[id='waterfall']").find("a")
        shotscreensNodes.each(function(index, element) {
            movieInfo.shotscreens.push($(element).attr("href"))
        })
        return movieInfo
    }
    async GetPersonInfo(name) { }
    async ParsePersonInfo(name, content) { }
}
module.exports = new ProviderTest2()