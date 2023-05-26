const express = require('express')
const multipart = require('multer');
const webSocket = require('ws')
const path = require('path')
const { logger } = require('weimingcommons')
class net {
    constructor() {
        this.events = {}
        this.clients = []
    }
    async init() {
        let app = express()
        app.use(express.json())
        app.use(express.text())
        app.use(multipart({ dest: "temp" }).any())                //设置上传文件存放的地址
        app.use("/client", express.static(path.join(__dirname, "client")))
        app.use("/music", express.static(path.join(__dirname, "data/music")))
        app.use("*", (_req, res, next) => {
            res.header('Access-Control-Allow-Origin', '*');
            res.header("Access-Control-Allow-Headers", "Content-Type,Content-Length, Authorization, Accept,X-Requested-With");
            res.header("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS");
            next()
        })
        app.get("/", (_req, res) => {
            res.writeHead(301, { 'Location': '/client' });
            res.end();
        })
        app.post("/execute", async (req, res) => {
            let code = req.body.code;
            let data = req.body.data;
            logger.info(`===> [${req.ip}] execute [${code}] : ${JSON.stringify(data)}`)
            try {
                let msgData = await this.fireFunc(code, data, req, res, null)
                logger.info(`<=== [${req.ip}] execute [${code}] : ${msgData}`)
            } catch (e) {
                this.notice("error", `execute is error, from:${req.ip}  ${code} - ${JSON.stringify(data)} : ${e.stack}`)
            }
            res.end();
        })
        let port = 4100
        let server = app.listen(port, () => {
            console.log(`应用正在监听 http://127.0.0.1:${port}`);
        })
        new webSocket.Server({ server: server }).on("connection", (ws) => {
            this.clients.push(ws)
            logger.info(`有链接 ${ws._socket.remoteAddress} 进入, 当前总数量 : ${this.clients.length}`)
            ws.onclose = () => {
                let index = this.clients.indexOf(ws)
                if (index >= 0) {
                    this.clients.splice(index, 1)
                }
                logger.info(`有链接断开, 当前总数量 : ${this.clients.length}`)
            }
        })
    }
    async fireFunc(code, data, req, res) {
        let evt = this.events[code]
        let msgData = undefined
        if (evt) {
            let result = await evt(data, req, res, code)
            if (result) {
                msgData = typeof (result) == "string" ? result : JSON.stringify(result)
            }
        }
        if (msgData) {
            res.write(msgData)
            if (msgData.length > 256) {
                msgData = ""
            }
        } else {
            msgData = ""
        }
        return msgData
    }
    register(code, func) {
        this.events[code] = func
    }
    sendMessage(code, data) {
        try {
            let clients = this.clients.concat()
            for (let ws of clients) {
                if (!ws) { continue; }
                try {
                    ws.send(JSON.stringify({ code: code, data: data }))
                } catch (e) {
                    logger.error("sendMessage is error : ", e)
                }
            }
        } catch (e) {
            logger.error("sendMessage is error : ", e)
        }
    }
    notice() {

    }
}
module.exports = new net()