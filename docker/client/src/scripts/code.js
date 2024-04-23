const ConfigType = {
    CommandConfig: "CommandConfig",
}
const RequestCode = {
    GetConfig: "GetConfig",             //请求配置
    SetConfig: "SetConfig",             //保存配置
    DelConfig: "DelConfig",             //删除配置
    GetCommandList: "GetCommandList",   //请求配置列表
}

const Result = {
    ConfigType,
    RequestCode
}

export default Result
//     RequestCode : {
//         GetConfig: "GetConfig",             //请求配置
//         SetConfig: "SetConfig",             //保存配置
//         DelConfig: "DelConfig",             //删除配置
    
//         GetCommandList: "GetCommandList",   //请求配置列表
    
    
//         requestCommand: "requestCommand",                   //请求命令
//         saveCommand: "saveCommand",                         //保存命令
//         deleteCommand: "deleteCommand",                     //删除命令
    
//         requestServerInfo: "requestServerInfo",             //请求服务器设备信息
//         setServerInfo: "setServerInfo",                     //设置服务器信息
    
//         executeCommand: "executeCommand",                   //添加一个任务(完成)
        
//         requestHistorys: "requestHistorys",                 //请求历史记录(完成)
//         requestReleases: "requestReleases",                 //请求发布列表(完成)
//         deleteHistory: "deleteHistory",                     //删除一条历史纪录(完成)
//         releaseHistory: "releaseHistory",                   //发布一个历史记录(完成)
//         cancelRelease: "cancelRelease",                     //取消一条发布信息(完成)
//         historyInfo: "historyInfo",                         //请求历史记录详细信息(完成)
//         historyLog: "historyLog",                           //请求历史记录详细信息(完成)
//         releaseInfo: "releaseInfo",                         //请求版本信息(完成)
//         addRelease: "addRelease",                           //添加一个Release
//         setRelease: "setRelease",                           //修改Release名字
//         releaseList: "releaseList",                         //请求release列表
//         operateHistory: "operateHistory",                   //操作History
//         operateHistoryNew: "operateHistoryNew",             //操作History
    
//         backupProjects: "backupProjects",                   //备份工程
//         deleteProjects: "deleteProjects",                   //删除工程文件
//         moveToNas: "moveToNas",                             //迁移生成文件到NAS
//         backupLogToNAS : "backupLogToNAS",                  //备份日志文件到NAS
    
//         buildChannel: "buildChannel",                       //生成渠道包
    
//         requestCommits: "requestCommits",                   //请求前端提交记录
//         deleteCommits: "deleteCommits",                     //删除某个提交记录
    
//         updateReleaseNote: "updateReleaseNote",             //更新 ReleaseNote 文件
//         parseReleaseNote: "parseReleaseNote",               //解析 ReleaseNote 文件(完成)
//         uploadReleaseNote: "uploadReleaseNote",             //上传 ReleaseNote(完成)
    
//         parseIOSCrashFile: "parseIOSCrashFile",             //解析IOS Crash文件(完成)
//         parseAndroidNative: "parseAndroidNative",           //解析Android Native函数(完成)
    
    
//         requestClientLogs: "requestClientLogs",             //请求客户端日志(完成)
//         addClientLog: "addClientLog",                       //新增一条客户端日志(完成)
    
//         requestProfiles: "requestProfiles",                 //请求描述文件(完成)
//         deleteProfile: "deleteProfile",                     //删除描述文件(完成)
//         uploadProfile: "uploadProfile",                     //上传描述文件(完成)
//         requestUrl: "requestUrl",                           //网络请求(完成)
//         uploadFile: "uploadFile",                           //上传文件
    
//         queryEntry: "queryEntry",                           //请求Entry
//         saveEntry: "saveEntry",                             //保存Entry
//         setEntryTools: "setEntryTools",                     //设置Entry SignKey和维护
//         downloadVersion: "downloadVersion",
//         assetsCompare: "assetsCompare",                     //资源比较
//         triggerCompare: "triggerCompare",                   //Trigger比较
//         mapVerCompare: "mapVerCompare",                     //Map版本比较
//         mapCompare: "mapCompare",                           //Map比较
//         scriptCompare: "scriptCompare",                     //脚本比较
    
//         uploadAdsAssets: "uploadAdsAssets",                 //上传广告素材
    
//         refreshEntryFile: "refreshEntryFile",               //刷新EntryFile
//         getAuthList: "getAuthList",                         //请求权限列表
//         setAuth: "setAuth",                                 //设置权限
//         createBranch: "createBranch",                       //新建分支
//         cleanUnityLibrary : "cleanUnityLibrary",            //清除Unity的Library  包括 MapEditor androiddevelop iosdevelop
//         setClipboard: "setClipboard",                       //设置剪切板
//         getClipboard: "getClipboard",                       //获取剪切板
//         getAllClipboard: "getAllClipboard",                 //获取剪切板
//         deviceInfo: "deviceInfo",                           //设备信息
//         requestDeviceInfo: "requestDeviceInfo",             //
//     }
// }
// module.exports.ConfigType = {
//     CommandConfig: "CommandConfig",
// }
// module.exports.RequestCode = {
//     GetConfig: "GetConfig",             //请求配置
//     SetConfig: "SetConfig",             //保存配置
//     DelConfig: "DelConfig",             //删除配置

//     GetCommandList: "GetCommandList",   //请求配置列表


//     requestCommand: "requestCommand",                   //请求命令
//     saveCommand: "saveCommand",                         //保存命令
//     deleteCommand: "deleteCommand",                     //删除命令

//     requestServerInfo: "requestServerInfo",             //请求服务器设备信息
//     setServerInfo: "setServerInfo",                     //设置服务器信息

//     executeCommand: "executeCommand",                   //添加一个任务(完成)
    
//     requestHistorys: "requestHistorys",                 //请求历史记录(完成)
//     requestReleases: "requestReleases",                 //请求发布列表(完成)
//     deleteHistory: "deleteHistory",                     //删除一条历史纪录(完成)
//     releaseHistory: "releaseHistory",                   //发布一个历史记录(完成)
//     cancelRelease: "cancelRelease",                     //取消一条发布信息(完成)
//     historyInfo: "historyInfo",                         //请求历史记录详细信息(完成)
//     historyLog: "historyLog",                           //请求历史记录详细信息(完成)
//     releaseInfo: "releaseInfo",                         //请求版本信息(完成)
//     addRelease: "addRelease",                           //添加一个Release
//     setRelease: "setRelease",                           //修改Release名字
//     releaseList: "releaseList",                         //请求release列表
//     operateHistory: "operateHistory",                   //操作History
//     operateHistoryNew: "operateHistoryNew",             //操作History

//     backupProjects: "backupProjects",                   //备份工程
//     deleteProjects: "deleteProjects",                   //删除工程文件
//     moveToNas: "moveToNas",                             //迁移生成文件到NAS
//     backupLogToNAS : "backupLogToNAS",                  //备份日志文件到NAS

//     buildChannel: "buildChannel",                       //生成渠道包

//     requestCommits: "requestCommits",                   //请求前端提交记录
//     deleteCommits: "deleteCommits",                     //删除某个提交记录

//     updateReleaseNote: "updateReleaseNote",             //更新 ReleaseNote 文件
//     parseReleaseNote: "parseReleaseNote",               //解析 ReleaseNote 文件(完成)
//     uploadReleaseNote: "uploadReleaseNote",             //上传 ReleaseNote(完成)

//     parseIOSCrashFile: "parseIOSCrashFile",             //解析IOS Crash文件(完成)
//     parseAndroidNative: "parseAndroidNative",           //解析Android Native函数(完成)


//     requestClientLogs: "requestClientLogs",             //请求客户端日志(完成)
//     addClientLog: "addClientLog",                       //新增一条客户端日志(完成)

//     requestProfiles: "requestProfiles",                 //请求描述文件(完成)
//     deleteProfile: "deleteProfile",                     //删除描述文件(完成)
//     uploadProfile: "uploadProfile",                     //上传描述文件(完成)
//     requestUrl: "requestUrl",                           //网络请求(完成)
//     uploadFile: "uploadFile",                           //上传文件

//     queryEntry: "queryEntry",                           //请求Entry
//     saveEntry: "saveEntry",                             //保存Entry
//     setEntryTools: "setEntryTools",                     //设置Entry SignKey和维护
//     downloadVersion: "downloadVersion",
//     assetsCompare: "assetsCompare",                     //资源比较
//     triggerCompare: "triggerCompare",                   //Trigger比较
//     mapVerCompare: "mapVerCompare",                     //Map版本比较
//     mapCompare: "mapCompare",                           //Map比较
//     scriptCompare: "scriptCompare",                     //脚本比较

//     uploadAdsAssets: "uploadAdsAssets",                 //上传广告素材

//     refreshEntryFile: "refreshEntryFile",               //刷新EntryFile
//     getAuthList: "getAuthList",                         //请求权限列表
//     setAuth: "setAuth",                                 //设置权限
//     createBranch: "createBranch",                       //新建分支
//     cleanUnityLibrary : "cleanUnityLibrary",            //清除Unity的Library  包括 MapEditor androiddevelop iosdevelop
//     setClipboard: "setClipboard",                       //设置剪切板
//     getClipboard: "getClipboard",                       //获取剪切板
//     getAllClipboard: "getAllClipboard",                 //获取剪切板
//     deviceInfo: "deviceInfo",                           //设备信息
//     requestDeviceInfo: "requestDeviceInfo",             //
// }
// module.exports.MsgCode = {
//     notice: "notice",                           //通知
//     write: "write",                             //日志
//     log: "log",                                 //日志
//     refreshHistory: "refreshHistory",           //刷新单条历史记录
//     refreshHistorys: "refreshHistorys",         //刷新历史记录列表
//     refreshReleases: "refreshReleases",         //刷新发布历史记录列表
//     addClientConfig: "addClientConfig",         //新增一条客户端配置
// }
// //操作历史记录
// module.exports.HistoryOperate = {
//     //纯服务器操作
//     updateInfo: "updateInfo",                   //更新数据
//     uploadFile: "uploadFile",                   //上传其他文件
//     //客户端请求操作
//     uploadApplication: "uploadApplication",     //上传Apk&Ipa到CDN
//     uploadAssetsCDN: "uploadAssetsCDN",         //上传资源到CDN
//     uploadAssetsNAS: "uploadAssetsNAS",         //上传资源到NAS
//     updateEntry: "updateEntry",                 //更新Entry到此版本
//     generateEnterIpa: "generateEnterIpa",       //生成企业签名包
//     uploadAppStore: "uploadAppStore",           //上传ipa到AppStore
//     uploadDSYM: "uploadDSYM",                   //上传符号文件dSYM
//     createAdhocInstall: "createAdhocInstall",   //生成IOS adhoc 远程安装
//     createEnterInstall: "createEnterInstall",   //生成IOS enter 远程安装
//     uploadGooglePlay: "uploadGooglePlay",       //上传apk到GooglePlay
//     uploadApk: "uploadApk",                     //上传热更apk
//     splitPackge: "splitPackge"                  //生成分包
// }