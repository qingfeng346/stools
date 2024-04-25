const ConfigType = {
    ServerConfig: "ServerConfig",
    CommandConfig: "CommandConfig",
}
const RequestCode = {
    GetConfigList: "GetConfigList",     //请求配置列表
    GetConfig: "GetConfig",             //请求配置
    SetConfig: "SetConfig",             //保存配置
    DelConfig: "DelConfig",             //删除配置
    GetCommandList: "GetCommandList",   //请求配置列表
}
const Result = {
    ConfigType,
    RequestCode
}

module.exports = Result