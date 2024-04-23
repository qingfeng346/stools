const express = require('express')
const multipart = require('multer');
const webSocket = require('ws')
const path = require('path')
const { logger, FileUtil } = require('weimingcommons')
class net {
    constructor() {
        this.events = {}
        this.clients = []
        logger.ilog = this
    }
    write(str) {
        process.stdout.write(str)
        this.sendMessage("write", str)
    }
    writeError(str) {
        process.stderr.write(str)
        this.sendMessage("write", str)
    }
    info(str) {
        console.log(str)
        this.sendMessage("log", str)
    }
    warn(str) {
        console.warn(str)
        this.sendMessage("log", str)
    }
    error(str) {
        console.error(str)
        this.sendMessage("log", str)
    }
    notify(str) {
        console.log(str)
        this.sendMessage("notice", { type: "info", msg: str })
    }
    notifySuccess(str) {
        console.log(str)
        this.sendMessage("notice", { type: "success", msg: str })
    }
    notifyError(str) {
        console.error(str)
        this.sendMessage("notice", { type: "error", msg: str })
    }
    async init() {
        FileUtil.CreateDirectory("./music")
        let app = express()
        app.all("*", (_req, res, next) => {
            res.header('Access-Control-Allow-Origin', '*');
            res.header('Access-Control-Allow-Credentials', 'true');
            res.header("Access-Control-Allow-Headers", "Content-Type,Content-Length, Authorization, Accept,X-Requested-With");
            res.header("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS");
            next()
        })
        app.use(express.json())
        app.use(express.text())
        app.use(multipart({ dest: "temp" }).any())                //设置上传文件存放的地址
        app.use("/client", express.static(path.join(__dirname, "client")))
        app.use("/music", express.static(path.join(__dirname, "music")))
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
                this.notifyError(`execute is error, from:${req.ip}  ${code} - ${JSON.stringify(data)} : ${e.stack}`)
            }
            res.end();
        })
        let port = 4100
        let server = app.listen(port, () => {
            console.log(`应用正在监听 http://127.0.0.1:${port}`);
        })
        new webSocket.Server({ server: server }).on("connection", (ws) => {
            this.clients.push(ws)
            ws.onclose = () => {
                let index = this.clients.indexOf(ws)
                if (index >= 0) {
                    this.clients.splice(index, 1)
                }
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
}
module.exports = new net()