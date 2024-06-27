const ActorManager = require("../Manager/ActorManager")
const MovieManager = require("../Manager/MovieManager")
const message = require("../message")
const { RequestCode } = require("../code")
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
    async GetMovieList(data) {
        return await MovieManager.GetMovieList(data.type, data.value, data.page, data.pageSize)
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