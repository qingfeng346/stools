const utils = require('../utils');
const cheerio = require('cheerio');
class ProviderTest2 {
    async GetMovieInfo(name) {
        var url = `https://jav5.land/ja/id_search.php?keys=${name}`;
        let result = await utils.get(url)
        if (result == null) return
        let $ = cheerio.load(result.data)
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
        let shotscreensNodes = $("span[id='waterfall']").find("a")
        shotscreensNodes.each(function(index, element) {
            movieInfo.shotscreens.push($(element).attr("href"))
        })
        return movieInfo
    }
    async GetPersonInfo(name) {
//         var url = `https://www.avbase.net/talents/${name}`;
//         let result  = await utils.get(url)
//         let $ = cheerio.load(result.data)
//         let jsonStr = $($("script[id='__NEXT_DATA__']").first()).text()
//         let jsonData = JSON.parse(jsonStr).props.pageProps.talent;
//         let personInfo = {
//             imageUrl : jsonData.primary.image_url,
//         }
//         if (jsonData?.primary?.meta != null) {
//             let fanza = jsonData?.primary?.meta.fanza
//             personInfo.desc = `出生地:${fanza.prefectures}
// 身高:${fanza.height}cm
// 罩杯:${fanza.cup}`;
//         }
        // return personInfo
    }
}
module.exports = new ProviderTest2()