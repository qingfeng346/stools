const utils = require('../utils');
const cheerio = require('cheerio');
class ProviderTest {
    async GetMovieIdByName(name) {
        let result = await utils.get(`https://www.avbase.net/works?q=${name}`)
        let $ = cheerio.load(result.data)
        let nodes = $("a[class='text-md font-bold btn-ghost rounded-lg m-1 line-clamp-5']")
        let results = []
        if (nodes != null) {
            nodes.each((index, element) => {
                let url = $(element).attr("href")
                results.push(url.substring(url.lastIndexOf("/") + 1))
            })
        }
        if (results.length > 0)
            return results[0]
    }
    async GetMovieInfo(name) {
        let movieId = await this.GetMovieIdByName(name)
        if (movieId == null) return
        let url = `https://www.avbase.net/works/${movieId}`
        return await this.ParseMovieInfo(name, (await utils.get(url)).data)
    }
    async ParseMovieInfo(name, content) {
        if (content == null) return
        let $ = cheerio.load(content)
        let jsonStr = $($("script[id='__NEXT_DATA__']").first()).text()
        let jsonData = JSON.parse(jsonStr).props.pageProps.work;
        let product = jsonData.products[0];
        let movieInfo = {
            movieId : jsonData.work_id,
            title: jsonData.title,
            releaseDate: new Date(jsonData.min_date),
            imageUrl : product.image_url,
            thumbUrl : product.thumbnail_url,
            actors: [],
            tags: [],
            makers: [],
            labels: [],
            series: [],
            shotscreens: []
        }
        if (jsonData.casts != null) {
            for (let v of jsonData.casts) {
                movieInfo.actors.push(v.actor.name)
            }
        }
        if (jsonData.genres != null) {
            for (var v of jsonData.genres) {
                movieInfo.tags.push(v.name)
            }
        }
        if (product.maker != null) {
            movieInfo.makers.push(product.maker.name)
        }
        if (product.label != null) {
            movieInfo.labels.push(product.label.name)
        }
        if (product.series != null) {
            movieInfo.series.push(product.series.name)
        }
        if (product.sample_image_urls != null) {
            for (let v of product.sample_image_urls) {
                movieInfo.shotscreens.push(v.l)
            }
        }
        return movieInfo
    }
    async GetPersonInfo(name) {
        var url = `https://www.avbase.net/talents/${name}`;
        return await this.ParsePersonInfo(name, (await utils.get(url)).data)
    }
    async ParsePersonInfo(name, content) {
        if (content == null) return
        let $ = cheerio.load(content)
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
}
module.exports = new ProviderTest()