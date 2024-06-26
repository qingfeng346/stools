const path = require('path')
const ClientPath = path.resolve(process.cwd(), "./client")          //静态文件保存目录
const UploadPath = path.resolve(process.cwd(), "./data/upload")     //文件上传临时目录 
const AssetsPath = path.resolve(process.cwd(), "./data/assets")     //静态文件保存目录
const HistorysPath = path.resolve(AssetsPath, "historys")           //历史记录保存目录
const CommandPath = path.resolve(process.cwd(), "../command")       //打包命令行目录
const Result = {
    ClientPath,
    UploadPath,
    AssetsPath,
    HistorysPath,
    CommandPath,
}
module.exports = Result