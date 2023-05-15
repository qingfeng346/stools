const express = require('express')
const multipart = require('multer');
const webSocket = require('ws')
const path = require('path')
class net {
    async init() {
        let app = express()
        app.use(express.json())
        app.use(express.text())
        app.use(multipart({ dest: "temp" }).any())                //设置上传文件存放的地址
        app.use("/client", express.static(path.join(__dirname, "client")))
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
            let body = req.body
            let a = 0;
            console.log(req.body)
            res.end()
            // let code = req.body.code;
            // let data = req.body.data;
            // logger.info(`===> [${req.ip}] execute [${code}] : ${JSON.stringify(data)}`)
            // try {
            //     let msgData = await this.fireFunc(code, data, req, res, null)
            //     if (msgData == null || msgData.length > 256) {
            //         msgData = ``;
            //     }
            //     logger.info(`<=== [${req.ip}] execute [${code}] : ${msgData}`)
            // } catch (e) {
            //     module.exports.Notice("error", `execute is error, from:${req.ip}  ${code} - ${JSON.stringify(data)} : ${e.stack}`)
            // }
            // res.end();
        })
        let port = 4100
        let server = app.listen(port, () => {
            console.log(`应用正在监听 http://127.0.0.1:${port}`);
        })
        new webSocket.Server({ server: server }).on("connection", (ws) => {
            this.onConnected(ws)
        })
    }
    onConnected(ws) {
        // this.clients.push(ws)
        // logger.info(`有链接 ${ws._socket.remoteAddress} 进入, 当前总数量 : ${this.clients.length}`)
        // ws.onclose = () => {
        //     let index = this.clients.indexOf(ws)
        //     if (index >= 0) {
        //         this.clients.splice(index, 1)
        //     } else {
        //         logger.error("数组内没有找到对象 ")
        //     }
        //     logger.info("有链接断开, 当前总数量 : " + this.clients.length)
        // }
    }
}
module.exports = new net()