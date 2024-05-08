import axios from 'axios';
import { logger, Event, Util } from 'weimingcommons';
class net {
    constructor() {
        this.event = new Event()
        this.hostName = window.location.hostname
        this.httpPort = window.location.port
        if (Util.IsDevelopment) {
            this.httpPort = 4100
        }
        this.startWebSocket()
    }
    get ServerUrl() {
        return `http://${this.hostName}:${this.httpPort}`
    }
    get ServerSocket() {
        return `ws://${this.hostName}:${this.httpPort}`
    }
    //开始连接服务器 websocket
    startWebSocket() {
        let url = this.ServerSocket
        let ws = new WebSocket(url)
        // logger.log(`开始连接 websocket : [${url}]`)
        ws.onopen = () => {
            logger.log(`连接 ${url} 成功`)
        }
        ws.onmessage = this.onMessage.bind(this)
        ws.onclose = (evt) => {
            // logger.log(`链接 ${url} 断开, 10 秒后重连 : ${evt.code}`)
            setTimeout(() => { this.startWebSocket() }, 5000)
        }
    }
    onMessage(evt) {
        logger.log(`收到消息 : ${evt.data}`)
        let message = JSON.parse(evt.data)
        let code = message.code
        let data = message.data
        this.event.fire(code, data, code)
    }
    registerMessage(code, func) {
        this.event.register(code, func)
    }
    //向服务器发送一个请求
    async execute(code, data) {
        let postData = { code: code, data: data }
        let strData = JSON.stringify(postData)
        try {
            console.log(`正在发送请求 [${code}] : ${strData}`)
            let result = await axios.post(`${this.ServerUrl}/execute`, postData)
            console.log(`请求返回 [${code}] : ${JSON.stringify(result.data)}`)
            return result.data
        } catch (e) {
            console.error(`发送请求出错 [${code}] : ${strData} : ${e}`)
        }
        return null
    }
    //上传一个文件给服务器
    async upload(code, data, files, uploadProgress) {
        let strData = JSON.stringify(data)
        try {
            let params = new FormData();
            for (let name in files) {
                params.append(name, files[name])
            }
            params.set("code", code)
            params.set("data", strData)
            let config = {
                headers: {
                    'Content-Type': 'multipart/form-data'
                },
                onUploadProgress: uploadProgress
            }
            console.log(`正在上传文件 [${code}] : [${strData}]`)
            let result = await axios.post(`${this.ServerUrl}/upload`, params, config)
            console.log(`请求返回 [${code}] : ${JSON.stringify(result.data)}`)
            return result.data
        } catch (e) {
            console.error(`上传文件出错 [${code}] : [${strData}] : ${e}`)
        }
        return null
    }
}
export default new net()