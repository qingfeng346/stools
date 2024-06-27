const ActorManager = require("../Manager/ActorManager")
const MovieManager = require("../Manager/MovieManager")
const { RequestCode } = require("../code")
const database = require("../database")
const message = require("../message")
class RequestManager {
    async init() {
        let codes = [
            RequestCode.UpdateMovieList,
            RequestCode.GetMovieList,
            RequestCode.GetMovieInfo,
            RequestCode.UpdateMoveInfo,
            RequestCode.GetPersonInfo,
            RequestCode.UpdatePersonInfo,
        ]
        for (let code of codes) {
            message.register(code, this[code].bind(this))
        }
    }
    async UpdateMovieList() {
        await MovieManager.UpdateMovieList()
    }
    async GetMovieList() {
        return await MovieManager.GetMovieList()
    }
    async GetMovieInfo(data) {
        return await MovieManager.GetMovieInfoById(data.id)
    }
    async UpdateMoveInfo(data) {
        MovieManager.UpdateMoveInfo(data.id)
    }
    async GetPersonInfo(data) {
        return await ActorManager.GetActorInfoById(data.id)
    }
    async UpdatePersonInfo(data) {
        ActorManager.UpdatePersonInfo(data.id)
    }
}
module.exports = new RequestManager()