import net from './net'
import code from '../scripts/code.js';
import { Util } from 'weimingcommons';
const { RequestCode } = code;
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
    //解析url
    getParam(key) {
        let href = location.href;
        let params = href.substring(href.lastIndexOf("?") + 1).split("&")
        for (let str of params) {
            let index = str.indexOf("=")
            if (index <= 0) { continue }
            let name = str.substring(0, index);
            if (name == key) {
                return str.substring(index + 1)
            }
        }
        return ""
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
    //刷新影片列表
    async UpdateMovieList() {
        return await net.execute(RequestCode.UpdateMovieList)
    }
    //获取影片列表
    async GetMovieList(type, value) {
        return await net.execute(RequestCode.GetMovieList, { type: type, value: value })
    }
    //获取影片信息
    async GetMovieInfo(id) {
        let movieInfo = await net.execute(RequestCode.GetMovieInfo, { id: id })
        if (movieInfo != null) {
            movieInfo.thumbUrl = this.GetImageUrl(movieInfo.thumbUrl)
            movieInfo.imageUrl = this.GetImageUrl(movieInfo.imageUrl)
        }
        return movieInfo
    }
    //刷新影片信息
    async UpdateMoveInfo(id) {
        return await net.execute(RequestCode.UpdateMoveInfo, { id: id })
    }
    //解析影片信息
    async ParseMoveInfo(id, type, content) {
        return await net.execute(RequestCode.ParseMovieInfo, { id: id, type: type, content: content })
    }
    //获取演员信息
    async GetPersonInfo(id) {
        let personInfo = await net.execute(RequestCode.GetPersonInfo, { id: id })
        if (personInfo != null) {
            personInfo.imageUrl = this.GetImageUrl(personInfo.imageUrl)
        }
        return personInfo
    }
    //刷新演员信息
    async UpdatePersonInfo(id) {
        return await net.execute(RequestCode.UpdatePersonInfo, { id: id })
    }
    //解析影片信息
    async ParsePersonInfo(id, type, content) {
        return await net.execute(RequestCode.ParsePersonInfo, { id: id, type: type, content: content })
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
    GetDefaultImage150x150() {
        return `${net.ServerUrl}/assets/images/150x150.png`
    }
    GetDefaultImage150x200() {
        return `${net.ServerUrl}/assets/images/150x200.png`
    }
    GetImageUrl(id) {
        return `${net.ServerUrl}/image?id=${id}`
    }
}
export default new util()