const { RequestCode } = require("../code")
const database = require("../database")
const message = require("../message")
class RequestManager {
    constructor() {
        message.register(RequestCode.GetMovieList, this.OnGetMovieList.bind(this))
        message.register(RequestCode.GetMovieInfo, this.OnGetMovieInfo.bind(this))
        message.register(RequestCode.GetPersonInfo, this.OnGetPersonInfo.bind(this))
    }
    init() {

    }
    async OnGetMovieList() {
        return await database.movie.findAll()
    }
    async OnGetMovieInfo(data) {
        return await database.movie.findOne({ where: {id: data.id}})
    }
    async OnGetPersonInfo(data) {
        return await database.actor.findOne({ where: {id: data.id}})
    }
}
module.exports = new RequestManager()