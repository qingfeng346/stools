import net from './net'
import code from '../scripts/code.js';
const { RequestCode } = code;

const base64 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@";
class util {
    init($Message, $Modal, homePage) {
        this.$Message = $Message
        this.$Modal = $Modal
        this.homePage = homePage
        this.configs = {}
        this.commands = {}
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
    async ExecuteCommand(name, data, files) {
        console.log("-------- " + files)
        await net.upload(RequestCode.ExecuteCommand, { name: name, data: data }, files)
    }
}
export default new util()