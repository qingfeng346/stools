import axios from 'axios';
import { logger } from 'weimingcommons';
class net {
    constructor() {
        this.messages = new Map()
    }
    get ServerUrl() {
        return `http://127.0.0.1:6000`
    }
    get ServerSocket() {
        return `ws://127.0.0.1:6000`
    }
    //开始连接服务器 websocket
    startWebSocket() {
        let url = this.ServerSocket
        let ws = new WebSocket(url)
        logger.log(`开始连接 websocket : [${url}]`)
        ws.onopen = () => { logger.log(`连接 ${url} 成功`) }
        ws.onmessage = this.onMessage.bind(this)
        ws.onclose = (evt) => {
            logger.log(`链接 ${url} 断开, 5 秒后重连 : ${evt.code}`)
            setTimeout(() => { this.startWebSocket() }, 5000)
        }
    }
    onMessage(evt) {
        logger.log(`收到消息 : ${evt.data}`)
        let message = JSON.parse(evt.data)
        let code = message.code
        let data = message.data
        this.messages.get(code)?.(data, code)
    }
    registerMessage(code, func) {
        this.messages.set(code, func)
    }
    //向服务器发送一个请求
    async request(code, data, files, uploadProgress) {
        let postData = { code: code, data: data }
        let strData = JSON.stringify(postData)
        try {
            let params = new FormData()
            let config = {}
            if (Array.isArray(files) && files.length > 0) {
                for (let file of files) { params.append(file.name, file) }
                config = {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    },
                    onUploadProgress: uploadProgress
                }
            }
            params.set("data", strData)
            console.log(`正在发送请求 [${code}] : ${strData}`)
            let result = await axios.post(`${this.ServerUrl}/execute`, params, config)
            console.log(`请求返回 [${code}] : ${JSON.stringify(result.data)}`)
            return result.data
        } catch (e) {
            console.error(`发送请求出错 [${code}] : ${strData} : ${e}`)
        }
        return null
    }
}
export default new net()