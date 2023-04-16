const { Util } = require('weimingcommons')
const express = require('express')
async function main() {
    let app = express()
    app.use("/client", express.static('client'))
    // app.use(bodyParser.json())
    // app.use(bodyParser.urlencoded({ extended: false }));
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
    app.listen(3000, () => {
        console.log('应用正在监听 http://127.0.0.1:3000');
    })
}
main()