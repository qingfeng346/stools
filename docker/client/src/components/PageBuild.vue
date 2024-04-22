<template>
  <div class="content">
    <Form :model="formData" label-position="left" :label-width="150">
      <FormItem label="命令">
        <RadioGroup v-model="command" @on-change="OnChangeCommand" >
          <Radio v-for="command in commandList" :key="command" :label="command">{{ command }}</Radio>
        </RadioGroup>
      </FormItem>
      <div v-for="arg in args" :key="arg.name">
        <FormItem v-if="IsShow(arg)" :label="arg.label">
          <RadioGroup v-if="arg.type =='radio'" v-model="formData[arg.name]" @on-change="UpdateCommand">
            <Radio v-for="item in GetOptionList(arg)" :key="item.name" :label="item.name">{{ item.label }}</Radio>
          </RadioGroup>
          <CheckboxGroup v-else-if="arg.type == 'checkbox'" v-model="formData[arg.name]" @on-change="UpdateCommand">
            <Checkbox v-for="item in GetOptionList(arg)" :key="item.name" :label="item.name">{{ item.label }}</Checkbox>
          </CheckboxGroup>
          <i-switch v-else-if="arg.type == 'switch'" v-model="formData[arg.name]" size="large" @on-change="UpdateCommand">
            <span slot="open">On</span>
            <span slot="close">Off</span>
          </i-switch>
          <Input v-else-if="arg.type == 'input'" v-model="formData[arg.name]" @on-change="UpdateCommand" />
          <span v-else-if="arg.type == 'flag'" v-for="option in GetOptionList(arg)">
            {{option.label}}
            <i-switch :value="IsFlag(formData[arg.name], option.value)" @on-change="OnChangeFlag(arg.name, option.value, $event)">
              <span slot="open">开</span>
              <span slot="close">关</span>
            </i-switch>
          </span>
        </FormItem>
      </div>
      <FormItem label="最终命令">
        <Input v-model="finishCommand" type="textarea" :rows="3" readonly />
      </FormItem>
      <FormItem label="提交">
        <Button type="primary" size="large" icon="ios-add-circle-outline" @click="OnClickAdd">
          <span>添加到执行队列</span>
        </Button>
      </FormItem>
    </Form>
  </div>
</template>
<script>
import code from '../scripts/code.js';
const { RequestCode, ConfigType } = code;
// import { RequestCode, ConfigType } from '../scripts/code.js';
import net from '../scripts/net';
import util from '../scripts/util';
export default {
  data() {
    return {
      formData : {},              //当前选中的命令参数
      commandConfig : {},         //全局命令参数
      params : {},                //当前显示的参数
      finishCommand : "",         //最终命令字符串
      command : "",               //当前选中的命令
      commandList : [],           //所有命令列表
      commandInfo : {},           //当前命令信息
      args: [],                   //当前命令参数
    };
  },
  mounted() {
    this.UpdateCommandList()
  },
  methods: {
    async UpdateCommandList() {
      this.commandConfig = await util.GetConfig(ConfigType.CommandConfig)
      let list = await net.request(RequestCode.GetCommandList)
      for (let v of list) {
        v.info = JSON.parse(v.info)
      }
      list.sort(function (a, b) {
        let v1 = a.info?.sort ?? 999
        let v2 = b.info?.sort ?? 999
        return v1 - v2
      });
      this.commandList = []
      for (let v of list) {
        if (this.IsShow(v?.info)) {
          this.commandList.push(v.name)
        }
      }
      if (this.commandList.length > 0) {
        let command = localStorage.getItem("LastCommand")
        this.command = this.commandList.indexOf(command) >= 0 ? command : this.commandList[0]
        await this.OnChangeCommand(this.command)
      }
    },
    async OnChangeCommand(name) {
      localStorage.setItem("LastCommand", this.command)
      let config = localStorage.getItem(`CommandCache_${name}`)
      let commandInfo = await util.GetCommandInfo(name)
      this.commandInfo = commandInfo.content
      let saveConfig = config == null ? {} : JSON.parse(config)
      this.formData = {}
      this.params = {}
      this.args = [...this.commandInfo.args]
      if (commandInfo.info.skipFlag == true && 
          commandInfo.execute.Execute != null &&
          commandInfo.execute.Execute.length > 1) {
        let skipFlag = { name: "skipFlag", label: "跳过执行列表", type: "flag", options: [] }
        for (let i = 0; i < commandInfo.execute.Execute.length; i++) {
          skipFlag.options.push({ label: `${commandInfo.execute.Execute[i]}(${i})`, value: i })
        }
        this.args.push(skipFlag)
      }
      for (let arg of this.args) {
        this.params[arg.name] = arg
        this.formData[arg.name] = this.GetArgValue(arg, saveConfig[arg.name])
      }
      this.UpdateCommand_impl()
    },
    GetArgValue(arg, value) {
      switch (arg.type) {
        case "radio": {
          let list = this.GetOptionList(arg)
          return list.find(item => item.name == value) != null ? value : (arg.def ?? list[0].name)
        }
        case "checkbox": {
          let list = this.GetOptionList(arg)
          let data = []
          if (value != null) {
            for (let v of value) {
              if (list.find(item => item.name == v) != null) {
                data.push(v)
              }
            }
          } else if (arg.def != null) {
            data = arg.def
          }
          return data
        }
        case "switch": {
          if (value == null) {
            return arg.def == null ? false : arg.def
          }
          return value
        }
        case "input": {
          if (value == null) {
            return arg.def == null ? "" : arg.def
          }
          return value
        }
        case "flag": {
          if (value == null) {
            return arg.def == null ? "" : arg.def
          }
          return value
        }
      }
      return undefined
    },
    GetOptionList(arg) {
      let options = typeof arg.options == "string" ? this.commandConfig[arg.options] : arg.options
      let result = []
      for (let option of options) {
        if (typeof option == "string") {
          result.push({name: option, label: option})
        } else if (this.CheckCondition(option.condition)) {
          result.push(option)
        }
      }
      return result
    },
    CheckCondition(condition) {
      if (util.CheckCondition(condition)) {
        if (condition == null) return true
        if (condition.commands != null && condition.commands.indexOf(this.command) < 0) return false
        if (condition["!commands"] != null && condition["!commands"].indexOf(this.command) >= 0) return false
        if (condition.func != null && this[condition.func] != null && !this[condition.func]()) return false
        return true;
      }
      return false;
    },
    
    IsShow(arg) {
      if (!this.CheckCondition(arg?.condition)) {
        return false
      }
      return true
    },
    IsCommand(arg) {
      return arg.isCommand == null || arg.isCommand == true
    },
    IsFlag(flag, value) {
      return util.HasBase64Flag(flag, value)
    },
    OnChangeFlag(name, value, state) {
      this.formData[name] = util.SetBase64Flag(this.formData[name], value, state)
      this.UpdateCommand()
    },
    GetValue(key) {
      let value = this.formData[key];
      if (typeof value == "object") {
        return value.join(",");
      }
      return value;
    },
    GetCommand(space) {
      let command = "";
      for (let key in this.formData) {
        if (!this.IsShow(this.params[key]) || !this.IsCommand(this.params[key])) { continue; }
        let value = this.GetValue(key)
        if (`${value}` == "") { continue; }
        command += `-${key} ${value}${space}`;
      }
      return command
    },
    async UpdateCommand() {
      await util.sleep(0.1)
      for (let arg of this.args) {
        this.formData[arg.name] = this.GetArgValue(arg, this.formData[arg.name])
      }
      this.UpdateCommand_impl()
      this.$forceUpdate()
    },
    UpdateCommand_impl() {
      localStorage.setItem(`CommandCache_${this.command}`, JSON.stringify(this.formData))
      this.finishCommand = `-operate ${this.command} ` + this.GetCommand(" ")
    },
    OnClickAdd() {
      let command = this.GetCommand("<br>")
      util.confirm({
        title: "添加到执行队列",
        content: `<p>${command}</p>`,
        okText: "添加到执行队列",
        cancelText: "取消",
        onOk: this.OnClickAddOK,
      });
    },
    async OnClickAddOK() {
      if (this.formData.updateEntry == true && this.IsShow(this.params["updateEntry"])) {
        util.confirmEntry(this.formData.platform, this.formData.environment, this.formData.unity, async (entryData, updateEntryList) => {
          let id = await this.ExecuteCommand_impl()
          await util.operateHistory(id, HistoryOperate.updateEntry, { entry: entryData.Name, updateEntryList: updateEntryList })
        })
      } else {
        await this.ExecuteCommand_impl()
      }
    },
    OnClickAddSchedule() {
      this.isShowSchedule = true
    },
    async OnClickAddScheduleOk() {
      let data = {}
      data.title = this.scheduleTitle ?? ""
      data.timeType = 0
      data.timeData = this.scheduleDate.valueOf().toString()
      data.taskType = 0
      let operateData = []
      for (let arg of this.args) {
        if (arg.name != "updateEntry" && this.formData[arg.name] == true && !this.IsCommand(arg)) {
          operateData.push({
            userId: net.user.id, 
            code : RequestCode.operateHistory, 
            data : {
              type: arg.name 
            }
          })
        }
      }
      data.taskData = JSON.stringify({
        postUrl: `${net.ServerUrl}/execute`,
        postData: JSON.stringify({
          userId: net.user.id,
          code: RequestCode.executeCommand,
          data: {
            command: `${this.finishCommand} -url ${net.ServerUrl}`
          }
        }),
        operateData: JSON.stringify(operateData)
      })
      data.username = util.user.id
      data.enabled = true
      let taksId = await util.schedulePost(`add`, data);
      util.successMessage(`添加定时任务成功 : ${taksId}`)
    },
    async ExecuteCommand_impl() {
        let id = await util.executeCommand(this.finishCommand);
        for (let arg of this.args) {
          if (arg.name != "updateEntry" && this.formData[arg.name] == true && !this.IsCommand(arg)) {
            await util.operateHistory(id, arg.name)
          }
        }
        return id
    },




    IsAndroid() {
      return this.formData.platform.toLowerCase() == "android"
    },
    IsIOS() {
      return this.formData.platform.toLowerCase() == "ios"
    },
    IsGooglePC() {
      return this.formData.environment == null || this.formData.environment.indexOf("googlepc") >= 0
    },
    IsAmazonOrGooglePC() {
      return this.formData.environment != null && (this.formData.environment.indexOf("googlepc") >= 0 || this.formData.environment.indexOf("amazon") >= 0)
    },
    IsNotGooglePC() {
      return this.formData.environment == null || this.formData.environment.indexOf("googlepc") < 0
    },
    IsNotAmazonOrGooglePC() {
      return !this.IsAmazonOrGooglePC()
    },
    IsUploadCDN() {
      return this.formData.uploadAssetsCDN == true
    },
  },
};
</script>