const { Util, FileUtil } = require('weimingcommons')
const path = require('path')
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
            let param = FileUtil.GetFileJson(jsonFile)
            await this.init(param)
            this.resultFile = param.ResultFile
            this.id = param.Id
            console.log(`====================开始执行命令 ${param.Name}====================`)
            console.log(`参数 : ${JSON.stringify(param, null, 2)}`)
            this.tempPath = `${Util.getTempPath()}/${this.id}`
            console.log(`临时操作目录 : ${this.tempPath}`)
            FileUtil.CreateDirectory(this.tempPath)
            let commandInfo = param.Execute
            if (commandInfo.Execute && commandInfo.Execute.length > 0) {
                console.log(`执行列表 ${JSON.stringify(commandInfo.Execute)}`)
                let length = commandInfo.Execute.length
                for (let i = 0; i < length; i++) {
                    let executeKey = commandInfo.Execute[i]
                    let execute = null
                    if (typeof executeKey == "string") {
                        execute = this.executes[executeKey]
                    } else {
                        execute = this.FormatExecute(executeKey)
                    }
                    console.log(`开始执行 ${i+1}/${length} ${JSON.stringify(execute)} : ${Util.NowTimeString}`)
                    if (execute.type == "stools") {
                        await this.execStools(execute, param)
                    }
                }
            }
            FileUtil.DeleteFolder(this.tempPath)
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
    async init(param) {
        this.executes = {}
        if (param.BuildConfig.Executes != null) {
            for (let key in param.BuildConfig.Executes) {
                let value = this.FormatExecute(param.BuildConfig.Executes[key])
                this.executes[key] = value
                console.log(`添加公共 execute ${key} : ${JSON.stringify(value)}`)
            }
        }
    }
    FormatExecute(execute) {
        return execute
    }
    async execStools(execute, param) {
        const stoolsPath = path.join(process.cwd(), "stools");
        let args = [execute.command]
        for (let key in param.Args) {
            args.push(`-${key}`)
            args.push(param.Args[key])
        }
        console.log(`运行stools命令 : ${JSON.stringify(args)}`)
        await Util.execAsync("dotnet", stoolsPath, args)
    }
}
module.exports = new Build()