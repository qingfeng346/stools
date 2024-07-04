const RequestCode = {
    UpdateMovieList: "UpdateMovieList",
    GetMovieList: "GetMovieList",
    GetMovieInfo: "GetMovieInfo",
    UpdateMoveInfo: "UpdateMoveInfo",
    ParseMovieInfo: "ParseMovieInfo",
    GetPersonInfo: "GetPersonInfo",
    UpdatePersonInfo: "UpdatePersonInfo",
    ParsePersonInfo: "ParsePersonInfo",

    GetConfigList: "GetConfigList",     //请求配置列表
    GetConfig: "GetConfig",             //请求配置
    SetConfig: "SetConfig",             //保存配置
    DelConfig: "DelConfig",             //删除配置

    SetStorage: "SetStorage",
    GetStorage: "GetStorage",
    DelStorage: "DelStorage",
    SyncDatabase: "SyncDatabase",
}
const Result = {
    RequestCode,
}
module.exports = Result