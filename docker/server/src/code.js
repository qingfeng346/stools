const ConfigType = {
    ServerConfig: "ServerConfig",
    CommandConfig: "CommandConfig",
}
const RequestCode = {
    GetConfigList: "GetConfigList",     //请求配置列表
    GetConfig: "GetConfig",             //请求配置
    SetConfig: "SetConfig",             //保存配置
    DelConfig: "DelConfig",             //删除配置
    GetCommandList: "GetCommandList",   //请求命令列表
    GetCommand: "GetCommand",           //请求命令配置
    SetCommand: "SetCommand",           //设置命令
    DelCommand: "DelCommand",           //删除命令
}
const Result = {
    ConfigType,
    RequestCode
}

module.exports = Result