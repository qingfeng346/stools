import events from 'events'
import net from './net'
import code from '../scripts/code.js';
const { RequestCode } = code;
const base64 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@";
class util {
    constructor() {
        // this.event = new events.EventEmitter()
        this.historys = new Map()
        window.onresize = () => { this.fireEvent("onResize") }
    }
    //开发者模式
    get IsDevelopment() { return process.env.NODE_ENV == "development" }
    init($Message, $Modal, homePage) {
        this.$Message = $Message
        this.$Modal = $Modal
        this.homePage = homePage
        this.configs = {}
        this.commands = {}
    }
    //注册一个event
    registerEvent(eventName, func) {
        // this.event.removeAllListeners(eventName)
        // this.event.on(eventName, func)
    }
    //触发一个event
    fireEvent(eventName, ...args) {
        // this.event.emit(eventName, args)
    }
    registerOnResize(page, datas) {
        let update = () => {
            let height = window.innerHeight
            for (let data of datas) {
                page[data.name] = height - data.value - 40
            }
        }
        update()
        this.registerEvent("onResize", update)
    }
    isNullOrEmpty(str) {
        return str == null || str == ""
    }
    miniJson(text) {
        return this.isNullOrEmpty(text) ? "" : JSON.stringify(eval(`(${text})`))
    }
    formatJson(text) {
        return this.isNullOrEmpty(text) ? "" : JSON.stringify(eval(`(${text})`), null, 4)
    }
    successMessage(info) {
        this.log(`successMessage : ${info}`)
        this.$Message.success(info)
    }
    errorMessage(info) {
        this.log(`errorMessage : ${info}`)
        this.$Message.error(info)
    }
    noticeInfo(msg) {
        this.log(`[Info]${msg}`)
        this.$Message.info({
            duration: 6,
            render: h => {
                return h('pre', [msg])
            }
        })
    }
    noticeSuccess(msg) {
        this.log(`[Success]${msg}`)
        this.$Message.success({
            duration: 6,
            render: h => {
                return h('pre', [msg])
            }
        })
    }
    noticeError(msg) {
        this.log(`[Error]${msg}`)
        this.$Message.error({
            duration: 6,
            render: h => {
                return h('pre', [msg])
            }
        })
    }
    log(msg) {
        console.log(msg)
        this.homePage.AddLog(msg)
    }
    confirmBox(title, content, onOk, closeable) {
        let config = {
            title: title,
            content: content,
            okText: "确定",
            cancelText: "关闭",
            closeable: closeable ?? true,
        }
        if (onOk != null) {
            config.onOk = onOk
        }
        this.confirm(config)
    }
    confirm(info) {
        if (info.closable == undefined) { info.closable = true }
        this.$Modal.confirm(info)
    }
    //休眠
    sleep(duration) {
        return new Promise((resolve) => {
            setTimeout(resolve, duration)
        })
    }
    hasBase64Flag(text, index) {
        let offset = Math.floor(index / 6);
        let mask = 1 << (index % 6);
        let number = text.length > offset ? base64.indexOf(text[offset]) : 0;
        return (number & mask) != 0;
    }
    setBase64Flag(text, index, flag) {
        let builder = text.split("")
        let offset = Math.floor(index / 6);
        let mask = 1 << (index % 6);
        for (let i = builder.length; i <= offset; ++i) {
            builder.push("0")
        }
        var number = base64.indexOf(builder[offset]);
        if (flag) {
            if ((number & mask) != 0) return text;
            number |= mask;
        } else if (!flag) {
            if ((number & mask) == 0) return text;
            number &= ~mask;
        }
        builder[offset] = base64[number];
        return builder.join("");
    }
    checkCondition(condition) {
        // if (condition == null) { return true }
        // let serverName = this.serverName
        // if (condition.servers != null && condition.servers.indexOf(serverName) < 0) return false
        // if (condition["!servers"] != null && condition["!servers"].indexOf(serverName) >= 0) return false
        // if (condition.excludeServers != null && condition.excludeServers.indexOf(serverName) >= 0) return false
        // let authName = this.user.authName
        // if (condition.users != null && condition.users.indexOf(authName) < 0) return false
        // if (condition["!users"] != null && condition["!users"].indexOf(authName) >= 0) return false
        return true
    }
    CleanConfigCache() {
        this.configs = {}
    }
    //获取Config
    async GetConfig(name, force) {
        if (!force && this.configs.hasOwnProperty(name)) {
            return this.configs[name]
        } else {
            return this.configs[name] = (await net.execute(RequestCode.GetConfig, { name: name }))
        }
    }
    //设置Config
    async SetConfig(name, value) {
        try {
            await net.execute(RequestCode.SetConfig, { name: name, value: this.miniJson(value)})
            this.configs[name] = JSON.parse(value)
            return true
        } catch(e) {
            this.errorMessage(`SetConfig[${name}],内容不是正确的JSON格式:${e}`);
        }
        return false
    }
    //删除配置
    async DelConfig(name) {
        await net.execute(RequestCode.DelConfig, { name: name })
        delete this.configs[name]
    }
    CleanCommandCache() {
        this.commands = {}
    }
    //获取Command列表
    async GetCommandList() {
        return await net.execute(RequestCode.GetCommandList)
    }
    //获取命令缓存
    async GetCommand(name, force) {
        if (!force && this.commands.hasOwnProperty(name)) {
            return this.commands[name]
        } else {
            let result = await net.execute(RequestCode.GetCommand, { name: name })
            if (this.isNullOrEmpty(result) || this.isNullOrEmpty(result?.content)) { return null }
            let commandInfo = {}
            commandInfo.content = JSON.parse(result.content)
            commandInfo.info = this.isNullOrEmpty(result.info) ? {} : JSON.parse(result.info)
            commandInfo.execute = this.isNullOrEmpty(result.execute) ? {} : JSON.parse(result.execute)
            commandInfo.operate = this.isNullOrEmpty(result.operate) ? {} : JSON.parse(result.operate)
            this.commands[name] = commandInfo
            return commandInfo
        }
    }
    //设置命令
    async SetCommand(name, value) {
        try {
            await net.execute(RequestCode.SetCommand, { 
                name: name,
                info: this.miniJson(value.info),
                content: this.miniJson(value.content),
                execute: this.miniJson(value.execute),
                operate: this.miniJson(value.operate),
            })
            this.commands[name] = {
                info: JSON.parse(value.info),
                content: JSON.parse(value.content),
                execute: JSON.parse(value.execute),
                operate: JSON.parse(value.operate),
            }
            return true
        } catch(e) {
            this.errorMessage(`SetCommand[${name}],内容不是正确的JSON格式:${e}`);
        }
        return false
    }
    //删除命令
    async DelCommand(name) {
        await net.execute(RequestCode.DelCommand, { name: name })
        delete this.configs[name]
    }
    //执行命令
    async ExecuteCommand(name, args, files) {
        await net.upload(RequestCode.ExecuteCommand, { name: name, args: args }, files)
    }

    //解析一条历史记录
    async parseHistory(rawData) {
        let data = null
        if (this.historys.has(rawData.id)) {
            data = this.historys.get(rawData.id)
        } else {
            data = {}
            this.historys.set(rawData.id, data)
        }
        for (let key in rawData) { data[key] = rawData[key] }
        let serverInfo = this.getServerInfo(rawData.serverAddress)
        let serverUrl = `http://${serverInfo?.address}:${serverInfo?.httpPort}`
        let command = new commandParser().parse(data.command.split(" "))
        let cdnUrl = (await this.GetConfig("CDNConfig")).url
        data.platform = command.get("-platform") ?? ""
        data.branch = command.get("-branch")
        data.operate = command.get("-operate")
        data.environment = command.get("-environment")
        data.uploadCDN = command.get("-uploadCDN")
        data.unityVersion = command.get("-unity")
        data.serverLabel = serverInfo?.label
        data.serverUrl = serverUrl
        data.statusText = this.getStatusText(data.status)
        data.isSuccess = data.status == Status.success
        data.isFail = data.status == Status.fail
        data.isBuildPlayer = data.operate == Operate.BuildPlayer
        data.isBuildAssets = data.operate == Operate.BuildBlueprints || data.operate == Operate.BuildAssetBundlesAndBlueprints || data.operate == Operate.BuildPlayer
        data.isAndroid = data.platform.toLowerCase() == "android"
        data.isIOS = data.platform.toLowerCase() == "ios"
        data.isProd = data.environment != null && data.environment.toLowerCase() == "prod"
        data.isGooglePC = data.environment != null && data.environment.toLowerCase() == "googlepc"
        data.isBackup = ((data.state & TaskState.backup) != 0)
        data.isUploadNAS = ((data.state & TaskState.uploadNAS) != 0)
        data.isUploadCDN = ((data.state & TaskState.uploadCDN) != 0)
        data.isUploadApplication = ((data.state & TaskState.uploadApplication) != 0)
        if (data.unityResult) {
            data.unityResult = typeof (data.unityResult) == "string" ? JSON.parse(data.unityResult) : data.unityResult
        }
        if (data.result) {
            data.result = typeof (data.result) == "string" ? JSON.parse(data.result) : data.result
        }
        data.uploadFiles = this.isNullOrEmpty(data.uploadFiles) ? {} : JSON.parse(data.uploadFiles)
        data.urls = this.isNullOrEmpty(data.urls) ? {} : JSON.parse(data.urls)
        data.logs = this.isNullOrEmpty(data.logs) ? [] : JSON.parse(data.logs)
        let results = []
        results.push({ name: "任务信息", label: `平台(${data.platform}) 环境(${data.environment}) 分支(${data.branch}) 操作(${data.operate}) 用户(${data.address})` })
        results.push({ name: "任务命令", label: data.command })
        if (data.endTime) {
            let startTime = this.toDateString(data.startTime)
            let endTime = this.toDateString(data.endTime)
            results.push({ name: "运行时间", label: `${startTime}  至  ${endTime}, 总耗时 ${this.toTimeString(data.endTime - data.startTime)}` })
        } else if (data.startTime) {
            results.push({ name: "运行时间", type: 2, time: data.startTime })
        }
        if (data.unityResult) {
            data.version = data.unityResult.Version
            results.push({ name: "App信息", label: `${data.unityResult.ProductName} (${data.unityResult.BundleID}), 版本号:${data.unityResult.Version}` })
        }
        let tags = [];
        if (data.releaseId != 0) { tags.push({ color: "warning", label: `已发布:${data.releaseId}` }) }
        if (data.isUploadCDN) { tags.push({ color: "#515a6e", label: `已上传到CDN` }) }
        if (data.isUploadNAS) { tags.push({ color: "#515a6e", label: `已上传到NAS` }) }
        if (data.isUploadApplication) { tags.push({ color: "#515a6e", label: `app已上传到CDN` }) }
        if (data.isBackup) { tags.push({ color: "primary", label: `已备份工程` }) }
        data.tags = tags
        if (data.isSuccess && tags.length > 0) {
            results.push({ name: "任务Tags", type: 5, tags: tags })
        }
        let consoleUrl = ""
        if (data.logPath == 0) {
            consoleUrl = `${serverUrl}/assets/historys/${rawData.id}/console.log`
        } else {
            consoleUrl = `${serverUrl}/assets/produces/AdventureReleased/historys/${rawData.id}/console.log`
        }
        results.push({ name: "执行日志", label: `<a href="${consoleUrl}" target="_blank"'>${consoleUrl}</a>` })
        for (let log of data.logs) {
            let url = `${net.ServerUrl}/assets/produces/AdventureReleased/historys/${rawData.id}/operate-${log}.log`
            results.push({ name: "操作日志", label: `<a href="${url}" target="_blank"'>${url}</a>` })
        }
        if (data.result?.gitInfo) {
            let gitInfo = data.result.gitInfo
            results.push({ name: "git最近提交信息", label: `<a href="http://dragonscapes.diandian.info:3000/ben.liu/Dragonscapes/src/${gitInfo.branch}" target="_blank">${gitInfo.branch}</a> | ${gitInfo.commitName} | ${this.toDateString(parseInt(gitInfo.commitTime) * 1000)} <<a href="http://dragonscapes.diandian.info:3000/ben.liu/Dragonscapes/commit/${gitInfo.hash}" target="_blank">${gitInfo.hash}</a>>` })
            results.push({ name: "git最近提交日志", label: gitInfo.commitMessage })
        }
        if (data.result) {
            results.push({ name: "执行结果", label: JSON.stringify(data.result, null, 2), type: 4 })
        }
        if (data.unityResult) {
            results.push({ name: "Unity返回", label: JSON.stringify(data.unityResult, null, 2), type: 4 })
        }
        let pathLink = `${serverUrl}/assets/produces/${data.savePath}/${data.projectPath}/`
        if (data.result?.FileNames) {
            let fileNames = data.result.FileNames
            results.push({ name: "下载地址", type: 1 })
            results.push({ name: "全部文件", type: 3, link: pathLink })
            for (let index in fileNames) {
                results.push({ name: index, type: 3, link: `${pathLink}${fileNames[index]}` })
                if (index == "ObbFile") {
                    results.push({ name: "obb自动安装", type: 3, link: `downloadobb://?url=${pathLink}${fileNames[index]}` })
                }
            }
        }
        let otherDownloadLabel = ""
        let uploadFiles = data.uploadFiles
        for (let key in uploadFiles) {
            let downloadFile = `${pathLink}${uploadFiles[key]}`
            otherDownloadLabel += `<a href="${downloadFile}" target="_blank">[${key}]</a>`
            if (downloadFile.endsWith("apk") || downloadFile.endsWith("ipa")) {
                if (downloadFile.endsWith("ipa"))
                    downloadFile = "https://da-cn.campfiregames.cn/public/install_ios.html?id=" + this.serverConfig.gameId + "_" + downloadFile.split("/output-")[1].split(".")[0]
                otherDownloadLabel += `<img src="https://api.qrserver.com/v1/create-qr-code/?size=100x100&data=${downloadFile}"/>`
            }
        }
        if (data.isUploadApplication) {
            otherDownloadLabel += "<br>"
            let fileNames = data.result?.FileNames
            if (fileNames) {
                for (let key in fileNames) {
                    let name = key.toLowerCase()
                    if (name.startsWith("apk") || name.startsWith("ipa")) {
                        otherDownloadLabel += `<a href="${cdnUrl}/public/install/${data.id}/${path.basename(fileNames[key])}" target="_blank">[CDN-${key}]</a>`
                    }
                }
            }
            for (let key in uploadFiles) {
                let downloadFile = `${cdnUrl}/public/install/${data.id}/${uploadFiles[key]}`
                otherDownloadLabel += `<a href="${downloadFile}" target="_blank">[CDN-${key}]</a>`
                if (data.isProd && (downloadFile.endsWith("apk") || downloadFile.endsWith("ipa"))) {
                    if (downloadFile.endsWith("ipa"))
                        downloadFile = "https://da-cn.campfiregames.cn/public/install_ios.html?id=" + this.serverConfig.gameId + "_" + downloadFile.split("/output-")[1].split(".")[0]
                    otherDownloadLabel += `<img src="https://api.qrserver.com/v1/create-qr-code/?size=100x100&data=${downloadFile}"/>`
                }
            }
        }
        if (otherDownloadLabel != "") {
            results.push({ name: "其他下载地址", label: otherDownloadLabel })
        }
        let otherUrlLabel = ""
        for (let key in data.urls) {
            let downloadFile = `${data.urls[key]}`
            otherUrlLabel += `<a href="${downloadFile}" target="_blank">[${key}]</a>`
            if (downloadFile.endsWith("apk") || downloadFile.endsWith("ipa")) {
                if (downloadFile.endsWith("ipa"))
                    downloadFile = "https://da-cn.campfiregames.cn/public/install_ios.html?id=" + this.serverConfig.gameId + "_" + downloadFile.split("/output-")[1].split(".")[0]
                otherUrlLabel += `<img src="https://api.qrserver.com/v1/create-qr-code/?size=100x100&data=${downloadFile}"/>`
            }
        }
        if (otherUrlLabel != "") {
            results.push({ name: "其他地址", label: otherUrlLabel })
        }

        if (data.releaseId != 0) {
            data.isRelease = true
            results.push({ name: "版本发布", type: 1 })
            results.push({ name: "版本链接", type: 3, link: `${net.ServerUrl}/#/home/releaseinfo?id=${data.releaseId}` })
            results.push({ name: "更新日志", label: `<pre>${data.releaseNote}</pre>` })
        } else {
            data.isRelease = false
        }
        data.infos = results
        return data
    }
}
export default new util()