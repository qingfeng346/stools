import MusicManager from "../Manager/MusicManager.js"
import message from "../message.js"
import { RequestCode } from "../code.js"
class RequestManager {
    async init() {
        let codes = [
            RequestCode.UpdateMusicList,
            RequestCode.GetMusicList,
            RequestCode.GetMusicInfo,
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
    async UpdateMusicList() {
        await MusicManager.UpdateMusicList()
    }
    async GetMusicList(data) {
        return await MusicManager.GetMusicList(data.page)
    }
    async GetMusicInfo(data) {
        return await MusicManager.GetMusicInfoById(data.id)
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