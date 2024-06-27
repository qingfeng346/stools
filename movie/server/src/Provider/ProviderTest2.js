const utils = require('../utils');
const cheerio = require('cheerio');
class ProviderTest {
    async GetMovieInfo(name) {
        let results = await this.SearchResult(name)
        if (results.length > 0)
            return await this.GetMovieInfoById(results[0])
    }
    async GetPersonInfo(name) {
        var url = `https://www.avbase.net/talents/${name}`;
        let result  = await utils.get(url)
        let $ = cheerio.load(result.data)
        let jsonStr = $($("script[id='__NEXT_DATA__']").first()).text()
        let jsonData = JSON.parse(jsonStr).props.pageProps.talent;
        let personInfo = {
            imageUrl : jsonData.primary.image_url,
        }
        if (jsonData?.primary?.meta != null) {
            let fanza = jsonData?.primary?.meta.fanza
            personInfo.desc = `出生地:${fanza.prefectures}
身高:${fanza.height}cm
罩杯:${fanza.cup}`;
        }
        return personInfo
    }
    async SearchResult(keyword) {
        let result = await utils.get(`https://www.avbase.net/works?q=${keyword}`)
        let $ = cheerio.load(result.data)
        let nodes = $("a[class='text-md font-bold btn-ghost rounded-lg m-1 line-clamp-5']")
        let results = []
        if (nodes != null) {
            nodes.each((index, element) => {
                let url = $(element).attr("href")
                results.push(url.substring(url.lastIndexOf("/") + 1))
            })
        }
        return results
    }
    async GetMovieInfoById(id) {
        let url = `https://www.avbase.net/works/${id}`
        let result = await utils.get(url)
        if (result == null) return
        let $ = cheerio.load(result.data)
        let jsonStr = $($("script[id='__NEXT_DATA__']").first()).text()
        let jsonData = JSON.parse(jsonStr).props.pageProps.work;
        let product = jsonData.products[0];
        let movieInfo = {
            movieId : jsonData.work_id,
            title: jsonData.title,
            imageUrl : product.image_url,
            thumbUrl : product.thumbnail_url,
            actors: [],
            tags: [],
            shotscreens: []
        }
        if (jsonData.casts != null) {
            for (let v of jsonData.casts) {
                movieInfo.actors.push(v.actor.name)
            }
        }
        if (product.sample_image_urls != null) {
            for (let v of product.sample_image_urls) {
                movieInfo.shotscreens.push(v.l)
            }
        }
        let tagNodes = $("a[class='rounded-lg border border-solid text-sm px-2 py-1']")
        tagNodes.each((index, element) => {
            movieInfo.tags.push($(element).text())
        })
        return movieInfo
    }
}
module.exports = new ProviderTest()