const ConfigType = {
    ServerConfig: "ServerConfig",       //服务器配置
    CommandConfig: "CommandConfig",     //命令配置
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
    ExecuteCommand: "ExecuteCommand",   //执行命令
}
const Result = {
    ConfigType,
    RequestCode
}

export default Result