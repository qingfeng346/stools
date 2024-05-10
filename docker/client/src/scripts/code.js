//任务状态
const Status = {
    Wait: "Wait",              //等待执行任务
    Process: "Process",        //正在执行任务
    Success: "Success",        //执行任务成功
    Fail: "Fail",              //任务执行失败
}
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
    GetHistorys: "GetHistorys",         //获取历史记录
    DelHistory: "DelHistory",           //删除历史记录
    SetStorage: "SetStorage",
    GetStorage: "GetStorage",
    DelStorage: "DelStorage",
}
const Result = {
    Status,
    ConfigType,
    RequestCode
}

export default Result