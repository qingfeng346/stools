import net from './net'
import code from '../scripts/code.js';
import { Util } from 'weimingcommons';
const { RequestCode, Status } = code;
const base64 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@";
class util {
    constructor() {
        this.events = {}
        this.historys = new Map()
        window.onresize = () => { 
            this.fireEvent("onResize")
        }
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
        this.events[eventName] = func
    }
    //触发一个event
    fireEvent(eventName, ...args) {
        let func = this.events[eventName]
        if (func != null) {
            func(...args)
        }
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
    formatJson(text, space) {
        return this.isNullOrEmpty(text) ? "" : JSON.stringify(eval(`(${text})`), null, space ?? 4)
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
    //获取历史记录
    async GetHistorys(page) {
        return await net.execute(RequestCode.GetHistorys, { page: page })
    }
    //删除历史记录
    async DelHistory(id) {
        await net.execute(RequestCode.DelHistory, { id: id })
    }
    //同步数据库
    async SyncDatabase() {
        await net.execute(RequestCode.SyncDatabase)
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
        let serverUrl = net.ServerUrl
        data.isSuccess = data.status == Status.Success
        data.isFail = data.status == Status.Fail
        if (data.result) {
            data.result = typeof (data.result) == "string" ? JSON.parse(data.result) : data.result
        }
        let results = []
        results.push({ name: "任务命令", label: data.name })
        if (data.endTime) {
            let startTime = Util.formatDate(new Date(data.startTime))
            let endTime = Util.formatDate(new Date(data.endTime))
            results.push({ name: "运行时间", label: `${startTime}  至  ${endTime}, 总耗时 ${Util.getElapsedTime(data.startTime, data.endTime)}` })
        } else if (data.startTime) {
            results.push({ name: "运行时间", type: 2, time: data.startTime })
        }
        let consoleUrl = `${serverUrl}/assets/historys/${rawData.id}/logs/console.log`
        results.push({ name: "执行日志", label: `<a href="${consoleUrl}" target="_blank"'>${consoleUrl}</a>` })
        results.push({ name: "命令参数", label: this.formatJson(data.args, 2), type: 4 })
        if (data.result) {
            results.push({ name: "执行结果", label: JSON.stringify(data.result, null, 2), type: 4 })
        }
        data.infos = results
        return data
    }

    async GetStorage(name) {
        return (await net.execute(RequestCode.GetStorage, { name: name }))?.value
    }
    async SetStorage(name, value) {
        await net.execute(RequestCode.SetStorage, { name: name, value: value})
    }
    async DelStorage(name) {
        await net.execute(RequestCode.DelStorage, { name: name })
    }
}
export default new util()