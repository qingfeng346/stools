const { Util, FileUtil } = require('weimingcommons')
const console = require('./logger')
class Build {
    async startBuild(jsonFile) {
        let startDate = new Date()
        try {
            this.results = {}
            //监听未捕获的异常
            process.on('uncaughtException', function(err, origin) {
                console.error(`未捕获的异常 ${err?.stack}\n${origin}`)
            })
            //监听Promise没有被捕获的失败函数
            process.on('unhandledRejection', function(err, promise){
                console.error(`未捕获的promise异常 ${err?.stack}`)
            })
            this.commandInfo = FileUtil.GetFileJson(jsonFile)
            this.resultFile = this.commandInfo.ResultFile
            this.id = this.commandInfo.Id
            console.log(`====================开始执行命令 ${this.commandInfo.Name}====================`)
            console.log(`命令信息 : ${JSON.stringify(this.commandInfo, null, 2)}`)
            let tempPath = `${Util.getTempPath()}/${this.id}`
            FileUtil.CreateDirectory(tempPath)
            console.log(`临时操作目录 : ${tempPath}`)
            FileUtil.DeleteFolder(tempPath)
            console.log(`执行成功`)
            this.saveSuccessResult()
        } catch (e) {
            console.error(`执行出错 : ${e?.stack}`)
            this.saveErrorResult(e.stack)
        }
        console.log(`====================命令执行完成,耗时 ${Util.getElapsedTime(startDate)}====================`)
    }
    saveSuccessResult() {
        this.results["code"] = 0
        FileUtil.CreateFileJson(this.resultFile, this.results)
    }
    saveErrorResult(error) {
        this.results = {}
        this.results["code"] = 1
        this.results["error"] = error
        FileUtil.CreateFileJson(this.resultFile, this.results)
    }
}
module.exports = new Build()