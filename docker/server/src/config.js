const path = require('path')
const HistorysPath = path.resolve(process.cwd(), "./data/historys")     //历史记录保存目录
const CommandPath = path.resolve(process.cwd(), "../command")           //打包命令行目录
const Result = {
    HistorysPath,
    CommandPath,
}
module.exports = Result