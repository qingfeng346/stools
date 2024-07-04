import MusicManager from "../Manager/MusicManager.js"
import message from "../message.js"
import { RequestCode } from "../code.js"
class RequestManager {
    async init() {
        let codes = [
            RequestCode.UpdateMovieList,
            RequestCode.GetMovieList,
            RequestCode.GetMovieInfo,
            RequestCode.UpdateMoveInfo,
            RequestCode.ParseMovieInfo,
            RequestCode.GetPersonInfo,
            RequestCode.UpdatePersonInfo,
            RequestCode.ParsePersonInfo,
        ]
        for (let code of codes) {
            message.register(code, this[code].bind(this))
        }
    }
    async UpdateMovieList() {
        await MusicManager.UpdateMovieList()
    }
    async GetMovieList(data) {
        return await MusicManager.GetMovieList(data.type, data.value, data.page, data.pageSize)
    }
    async GetMovieInfo(data) {
        return await MusicManager.GetMovieInfoById(data.id)
    }
    async UpdateMoveInfo(data) {
        MusicManager.UpdateMusicInfo(data.id)
    }
    async ParseMovieInfo(data) {
        MusicManager.ParseMovieInfo(data.id, data.type, data.content)
    }
    async GetPersonInfo(data) {
        return await ActorManager.GetActorInfoById(data.id)
    }
    async UpdatePersonInfo(data) {
        ActorManager.UpdatePersonInfo(data.id)
    }
    async ParsePersonInfo(data) {
        ActorManager.ParsePersonInfo(data.id, data.type, data.content)
    }
}
export default new RequestManager()