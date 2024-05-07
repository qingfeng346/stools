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
          <Upload v-else-if="arg.type == 'file'" type="drag" :paste="true" :name="arg.name" action :before-upload="CreateBeforeUploadFile(arg.name)">
            <div style="padding: 2px 0">
                <Icon type="ios-cloud-upload" size="40" style="color: #3399ff"></Icon>
                <div v-if="formFile[arg.name]">当前选择文件 : {{ formFile[arg.name].name }}</div>
                <div v-else>选择文件</div>
              </div>
          </Upload>
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
const { ConfigType } = code;
import util from '../scripts/util';
const StorageLastCommand = "PageBuild_StorageLastCommand"
const StorageCommandCache = "PageBuild_StorageCommandCache_";
export default {
  data() {
    return {
      formData : {},              //当前选中的命令参数
      formFile : {},              //当前选中的文件
      argInfos : {},              //所有参数的信息
      args: [],                   //当前命令参数
      commandConfig : {},         //全局命令参数
      commandList : [],           //所有命令列表
      command : "",               //当前选中的命令
      finishCommand : "",         //最终命令字符串
    };
  },
  mounted() {
    this.UpdateCommandList()
  },
  methods: {
    async UpdateCommandList() {
      util.CleanConfigCache()
      util.CleanCommandCache()
      this.commandConfig = await util.GetConfig(ConfigType.CommandConfig)
      let list = await util.GetCommandList()
      if (list == null) return;
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
        let command = localStorage.getItem(StorageLastCommand)
        this.command = this.commandList.indexOf(command) >= 0 ? command : this.commandList[0]
        await this.OnChangeCommand(this.command)
      }
    },
    async OnChangeCommand(name) {
      localStorage.setItem(StorageLastCommand, this.command)
      let commandInfo = await util.GetCommand(name)
      let saveConfigStr = localStorage.getItem(`${StorageCommandCache}${name}`)
      let saveConfig = saveConfigStr == null ? {} : JSON.parse(saveConfigStr)

      this.commandInfo = commandInfo.content
      
      this.formData = {}
      this.argInfos = {}
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
        this.argInfos[arg.name] = arg
        if (arg.type != "file")
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
      return util.checkCondition(condition)
    },
    IsShow(arg) {
      return this.CheckCondition(arg?.condition)
    },
    IsFlag(flag, value) {
      return util.hasBase64Flag(flag, value)
    },
    OnChangeFlag(name, value, state) {
      this.formData[name] = util.setBase64Flag(this.formData[name], value, state)
      this.UpdateCommand()
    },
    CreateBeforeUploadFile(name) {
      return (file) => {
        this.formFile[name] = file
        return false;
      }
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
        if (!this.IsShow(this.argInfos[key])) { continue; }
        let value = this.GetValue(key)
        if (`${value}` == "") { continue; }
        command += `-${key} ${value}${space}`;
      }
      return command
    },
    async UpdateCommand() {
      await util.sleep(0.1)
      for (let arg of this.args) {
        if (arg.type != "file")
          this.formData[arg.name] = this.GetArgValue(arg, this.formData[arg.name])
      }
      this.UpdateCommand_impl()
      this.$forceUpdate()
    },
    UpdateCommand_impl() {
      localStorage.setItem(`${StorageCommandCache}${this.command}`, JSON.stringify(this.formData))
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
      await util.ExecuteCommand(this.command, this.formData, this.formFile)
    }
  },
};
</script>