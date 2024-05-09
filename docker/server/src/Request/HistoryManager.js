const { Util, logger, FileUtil } = require("weimingcommons")
const { RequestCode, Status } = require("../code")
const { Op } = require("sequelize")
const message = require("../message")
const database = require("../database")
const ServerConfig = require("./ServerConfig")
const { HistorysPath, CommandPath } = require("../config")
class HistoryManager {
    constructor() {
        this.executingMap = {}
        message.register(RequestCode.ExecuteCommand, this.OnExecuteCommand.bind(this))
        message.register(RequestCode.GetHistorys, this.OnGetHistorys.bind(this))
    }
    async init() {

    }
    async CreateExecuteID() {
        while (true) {
            let nowDate = Util.NowDate
            let id = Util.getId(nowDate)
            let count = await database.history.count({ where: { id: id } })
            if (count == 0) {
                return { id: id, date: nowDate }
            }
            await Util.sleep(1000)
        }
    }
    //添加一条执行命令
    async AddExecute(name, args, files) {
        while (this.isCreateExecute) {
            await Util.sleep(100)
        }
        try {
            this.isCreateExecute = true
            let newID = await this.CreateExecuteID()
            let allFiles = {}
            for (let file of files) {
                allFiles[file.fieldname] = {
                    originalname: file.originalname,
                    path: file.path
                }
            }
            let data = {
                id: newID.id,
                createTime: newID.date,
                name: name,
                args: JSON.stringify(args),
                files: JSON.stringify(allFiles),
                status: Status.Wait,
                result: "",
            }
            return await database.history.create(data)
        } finally {
            this.isCreateExecute = false
        }
    }
    async OnExecuteCommand(data, files) {
        let execute = await this.AddExecute(data.name, data.args ?? {}, files ?? {})
        this.CheckExecuteCommand()
        return execute.id
    }
    async CheckExecuteCommand() {
        if (this.isExecuting) { return }
        this.isExecuting = true
        await this.ExecuteCommand(await database.history.findOne({ where: { status: Status.Wait }, order: ["createTime"] }))
    }
    async ExecuteCommand(execute) {
        if (execute == null) {
            this.isExecuting = false
            return
        }
        let id = execute.id
        try {
            execute.status = Status.Process
            execute.startTime = Util.NowDate
            await database.history.update({ status: execute.status, startTime: execute.startTime }, { where: { id: id } })
            logger.notify(`开始执行任务:${execute.name}\n任务ID : ${id}`)
            let hisotryPath = `${HistorysPath}/${id}`
            let historyLogFile = `${hisotryPath}/logs/console.log`
            let resultFile = `${hisotryPath}/result/result.json`
            let buildConfig = await ServerConfig.GetBuildConfig()
            let commandInfo = await ServerConfig.GetCommand(execute.name)
            let commandJson = {
                Id: id,
                Name: execute.name,
                Time: execute.createTime,
                Args: JSON.parse(execute.args),
                Files: JSON.parse(execute.files),
                ResultFile: resultFile,
                BuildConfig: buildConfig,
                Info: Util.isNullOrEmpty(commandInfo.info) ? {} : JSON.parse(commandInfo.info),
                Execute: Util.isNullOrEmpty(commandInfo.execute) ? buildConfig.Default : JSON.parse(commandInfo.execute),
            }
            let buildJsonFile = Util.getTempFile()
            await FileUtil.CreateFileJsonAsync(buildJsonFile, commandJson)
            let npmName = Util.IsWindows ? "npm.cmd" : "npm"
            let result = await Util.execAsyncLog(npmName, CommandPath, ["install"],   { encoding: Util.IsWindows ? "gbk" : "utf8", shell: true }, historyLogFile)
            if (result.code == 0) {
                await Util.execAsyncLog("node", CommandPath, ["index.js", buildJsonFile], { encoding: "utf8", shell: true }, historyLogFile, (sp) => { this.executingMap[id] = sp })
            }
            execute.endTime = Util.NowDate
            result = await FileUtil.GetFileJsonAsync(resultFile)
            if (result?.code == 0) {
                execute.status = Status.Success
                execute.result = JSON.stringify(result)
            } else {
                execute.status = Status.Fail
                execute.result = ""
            }
            await FileUtil.DeleteFileAsync(resultFile)
            FileUtil.DeleteEmptyFolder(hisotryPath)
            await database.history.update({ endTime: execute.endTime, status: execute.status, result: execute.result}, { where: { id: id } })
            if (execute.status == Status.Success) {
                logger.notifySuccess(`任务 : ${id} 执行成功`)
            } else {
                logger.notifyError(`任务 : ${id} 执行错误`)
            }
            for (let key in execute.files) {
                await FileUtil.DeleteFileAsync(execute.files[key])
            }
        } catch (e) {
            logger.error(`${id} ExecuteCommand is error : ${e.message}\n${e.stack}`)
        } finally {
            this.isExecuting = false
            this.executingMap[id] = null
        }
        this.CheckExecuteCommand()
    }
    async OnGetHistorys(data) {
        let pageSize = 20
        let page = data.page
        let searchValue = ""
        let condition = {
            limit: pageSize,
            offset: (page - 1) * pageSize,
            order: [["createTime", "DESC"]],
            where: {
                id: {
                    [Op.like]: `%${searchValue}%`
                }
            }
        }
        let total = await database.history.count(condition)
        let datas = await database.history.findAll(condition)
        let result = {}
        result.pageTotal = total
        result.pageSize = pageSize
        result.page = page
        result.datas = datas
        return result
    }
}
module.exports = new HistoryManager()