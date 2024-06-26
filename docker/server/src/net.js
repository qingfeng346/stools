const logger = require('log4js').getLogger('net.js')
const express = require('express')
const multipart = require('multer')
const webSocket = require('ws')
const path = require('path')
const message = require('./message')
const { UploadPath, AssetsPath, ClientPath } = require('./config')
class net {
    constructor() {
        this.clients = []
        require('weimingcommons').logger.ilog = this
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
        logger.info(str)
        this.sendMessage("log", str)
    }
    warn(str) {
        logger.warn(str)
        this.sendMessage("log", str)
    }
    error(str) {
        logger.error(str)
        this.sendMessage("log", str)
    }
    notify(str) {
        logger.info(`[Notice][info] ${str}`)
        this.sendMessage("notice", { type: "info", msg: str })
    }
    notifySuccess(str) {
        logger.info(`[Notice][success] ${str}`)
        this.sendMessage("notice", { type: "success", msg: str })
    }
    notifyError(str) {
        logger.error(`[Notice][error] ${str}`)
        this.sendMessage("notice", { type: "error", msg: str })
    }
    async init() {
        console.log(`AssetsPath : ${AssetsPath}`)
        console.log(`AssetsPath : ${ClientPath}`)
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
        app.use(multipart({ dest: UploadPath }).any())                              //设置上传文件存放的地址
        app.use("/assets", express.static(AssetsPath))
        app.use("/client", express.static(ClientPath))
        app.get("/", (_req, res) => {
            res.writeHead(301, { 'Location': '/client' });
            res.end();
        })
        app.post("/execute", async (req, res) => {
            let code = req.body.code;
            let data = req.body.data;
            logger.info(`===> [${req.ip}] execute [${code}] : ${JSON.stringify(data)}`)
            try {
                let msgData = await this.fireFunc(code, data, null, req, res)
                logger.info(`<=== [${req.ip}] execute [${code}] : ${msgData}`)
            } catch (e) {
                this.notifyError(`execute is error, from:${req.ip}  ${code} - ${JSON.stringify(data)} : ${e.message}\n${e.stack}`)
            }
            res.end();
        })
        app.post("/upload", async (req, res) => {
            let code = req.body.code
            let data = JSON.parse(req.body.data)
            logger.info(`===> [${req.ip}] upload [${code}] : ${JSON.stringify(data)}`)
            try {
                let msgData = await this.fireFunc(code, data, req.files, req, res)
                logger.info(`<=== [${req.ip}] upload [${code}] : ${msgData}`)
            } catch (e) {
                this.notifyError(`upload is error, from:${req.ip}  ${code} - ${JSON.stringify(data)} : ${e.message}\n${e.stack}`)
            }
            res.end()
        })
        let port = 4100
        let server = app.listen(port, () => {
            logger.info(`应用正在监听 http://127.0.0.1:${port}`);
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
    async fireFunc(code, data, files, req, res) {
        let evt = message.get(code)
        let msgData = undefined
        if (evt) {
            let result = await evt(data, files, code)
            if (result) {
                msgData = typeof (result) == "string" ? result : JSON.stringify(result)
            }
        } else {
            throw new Error(`找不到命令 : ${code}`)
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