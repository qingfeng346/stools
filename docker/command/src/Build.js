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
                        let { code, stdout, stderr } = await this.execStools(execute, param)
                        if (code != 0) {
                            throw new Error(`exec stools error, code:${code} stdout:${stdout} stderr:${stderr}`)
                        }
                    }
                }
            }
            console.log(`删除临时目录 : ${this.tempPath}`)
            await FileUtil.DeleteFolderAsync(this.tempPath)
            console.log(`执行成功`)
            this.saveSuccessResult()
        } catch (e) {
            console.error(`执行出错 : ${e?.message}\n${e?.stack}`)
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
        await this.DownloadFile("https://pics.dmm.co.jp/mono/actjpgs/syouda_tisato.jpg", "/app/root/root/音乐下载", "1111.jpg")
        const stoolsPath = path.join(process.cwd(), "stools");
        let args = ["run", execute.command]
        for (let key in param.Args) {
            args.push(`-${key}`)
            args.push(param.Args[key])
        }
        console.log(`运行stools命令:${stoolsPath} : ${JSON.stringify(args)}`)
        return await Util.execAsync("dotnet", stoolsPath, args)
    }
    DownloadFile = function(url, targetPath, fileName) {
        return new Promise((resolve, reject) => {
            let file = `${targetPath}/${fileName}`
            let percent = 0
            let time = new Date().getTime();
            let d = download(url)
            d.addListener("downloadProgress", function (progress) {
                let now = new Date().getTime();
                if (now - time > 2000 && progress.percent - percent >= 0.01) {
                    time = now
                    percent = progress.percent
                    console.log(`文件${url}下载进度:${Math.floor(percent * 100)}%`)
                }
            })
            d.pipe(fs.createWriteStream(file))
                .on("close", function() {
                    console.log(`文件${url}下载完成`)
                    resolve()
                })
        })
    }
}
module.exports = new Build()