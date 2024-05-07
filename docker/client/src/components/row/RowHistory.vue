<template>
  <Content class="history">
    <Form label-position="left" :label-width="150">
      <FormItem label="任务ID">
        <span> {{ history.id }} </span>
        <Dropdown trigger="click" v-if="!history.isFail && isShowMoreOperate" @on-click="OnClickMore">
          <a href="javascript:void(0)" ><Icon type="ios-more" />更多操作</a>
          <DropdownMenu slot="list">
            <div v-for="item in operates" :key="item.label">
              <DropdownItem v-if="item.vif()"
                :name="item.label" 
                :divided=item.divided>
                {{ item.label }}
              </DropdownItem>
            </div>
          </DropdownMenu>
        </Dropdown>
      </FormItem>
      <div v-for="(info, index) in infos" :key="index">
        <Divider v-if="info.type == 1" size="small">{{ info.name }}</Divider>
        <FormItem v-else :label="info.name">
          <RowTime v-if="info.type == 2" :time="info.time" />
          <a v-else-if="info.type == 3" :href="info.link" target="_blank">{{ info.link }}</a>
          <span v-else-if="info.type == 4">
            <Input class="source" type="textarea" :rows="5" :value="info.label" readonly />
          </span>
          <span v-else-if="info.type == 5">
            <Tag v-for="(value, key) in info.tags" :color="value.color" :key="key">{{ value.label }}</Tag>
          </span>
          <span v-else v-html="info.label"></span>
        </FormItem>
      </div>
    </Form>
    <!-- 发布版本 -->
    <Modal title="发布版本" v-model="release.isShow" @on-ok="OnClickReleaseOK">
      <Select v-model="release.select">
        <Option v-for="item in release.list" :value="item.id" :key="item.id">
          {{ item.title }}
        </Option>
      </Select>
      <Input type="textarea" v-model="release.input" :autosize="{ minRows: 3, maxRows: 7 }" placeholder="请填写本次发布更新内容,没有可不填" />
    </Modal>
    <Modal :title="operateNew.title" v-model="operateNew.isShow" @on-ok="OnClickOperateNew">
      <Form :model="formData" label-position="left" :label-width="150">
        <div v-for="arg in operateNew.args" :key="arg.name">
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
      </Form>
    </Modal>
  </Content>
</template>
<script>
import RowTime from "./RowTime";
import util from "../../scripts/util";
import { RequestCode, HistoryOperate } from "../../scripts/code";
export default {
  components: { RowTime },
  props: {
    history: {},
  },
  data() {
    return {
      infos : [],                 //所有信息
      isShowMoreOperate : false,  //是否显示更多操作按钮
      operates : [],              //更多操作列表

      //
      operateNew : {
        isShow: false,
        title: "",
        args: [],
      },
      formData : {},

      release: {
        isShow: false,
        select: undefined,
        list: [],
        input: ""
      }
    }
  },
  mounted() {
    this.Init()
  },
  methods: {
    async Init() {
      while (this.history.operate == null) {
        await util.sleep(10)
      }
      this.infos = this.history.infos;
      if (this.history.isFail) { return }
      let commandInfo = (await util.GetCommandInfo(this.history.operate))?.operate
      if (commandInfo == null) { return }
      this.operates = []
      if (commandInfo.Release) {
        this.operates.push({
          type: 1,
          label: "发布版本",
          func: this.OnClickRelease.bind(this),
          vif: this.GetTrue
        })
        this.operates.push({
          type: 1,
          label: "取消发布",
          func: this.OnClickCancelRelease.bind(this),      
          vif: () => this.history.isRelease
        })
      }
      if (commandInfo.Operate != null) {
        let first = true
        for (let v of commandInfo.Operate) {
          let op = JSON.parse(JSON.stringify(v))
          if (!this.CheckCondition(op.condition)) {
            continue
          }
          if (!util.isNullOrEmpty(op.msg)) {
            op.type = 0
          } else if (!util.isNullOrEmpty(op.func)) {
            op.type = 1
            op.func = this[op.func].bind(this)
          }
          if (!util.isNullOrEmpty(op.vif)) {
            op.vif = this[op.vif].bind(this)
          } else {
            op.vif = this.GetTrue
          }
          if (first) {
            first = false
            op.divided = true
          }
          this.operates.push(op)
        }
      }
      if (commandInfo.OperateNew != null) {
        let first = true
        for (let v of commandInfo.OperateNew) {
          let op = JSON.parse(JSON.stringify(v))
          if (!this.CheckCondition(op.condition)) {
            continue
          }
          op.type = 2
          if (!util.isNullOrEmpty(op.vif)) {
            op.vif = this[op.vif].bind(this)
          } else {
            op.vif = this.GetTrue
          }
          if (first) {
            first = false
            op.divided = true
          }
          this.operates.push(op)
        }
      }
      this.isShowMoreOperate = this.operates.length > 0
    },
    GetTrue() {
      return true
    },
    async RefreshInfo() {
      await util.refreshHistory(this.history.id)
    },
    async OnClickMore(name) {
      for (let operate of this.operates) {
        if (operate.label == name) {
          if (operate.type == 0) {
            this.SendMessage(operate)
          } else if (operate.type == 1) {
            operate.func()
          } else if (operate.type == 2) {
            this.operateNew.isShow = true
            this.operateNew.title = `${this.history.id}  ${operate.label}(${operate.msg})`
            this.operateNew.args = operate.args
            this.operateNew.operate = operate
            this.formData = {}
            for (let arg of operate.args) {
              this.formData[arg.name] = this.GetArgValue(arg)
            }
          }
          return
        }
      }
    },
    //发布版本
    async OnClickRelease() {
      this.release.list = await util.GetReleaseList();
      if (this.history.releaseId) {
        this.release.select = this.history.releaseId;
      } else if (this.release.list.length > 0 && this.release.select == undefined) {
        this.release.select = this.release.list[0].id;
      }
      this.release.isShow = true;
      this.release.input = this.history.releaseNote;
      if (util.isNullOrEmpty(this.release.input)) {
        this.release.input = this.history.result?.gitInfo?.commitMessage
      }
    },
    //确认发布版本
    async OnClickReleaseOK() {
      if (this.release.select == null || this.release.select <= 0) {
        util.errorMessage("请先选择发布版本");
        return;
      }
      let data = await util.releaseHistory(this.history.id, this.release.select, this.release.input);
      if (data) {
        util.errorMessage(`发布失败 : ${data}`);
      } else {
        util.successMessage("发布成功");
        this.RefreshInfo();
      }
    },
    async OnClickCancelRelease() {
      await util.cancelRelease(this.history.id);
      this.RefreshInfo();
    },
    async OnClickOperateNew() {
      await util.operateHistoryNew(this.history.id, this.operateNew.operate.msg, this.formData)
    },
    GetOptionList(arg) {
      let options = arg.options
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
    IsShow(arg) {
      if (!this.CheckCondition(arg?.condition)) {
        return false
      }
      return true
    },
    CheckCondition(condition) {
      return util.CheckCondition(condition)
    },
    IsFlag(flag, value) {
      return util.HasBase64Flag(flag, value)
    },
    OnChangeFlag(name, value, state) {
      this.formData[name] = util.SetBase64Flag(this.formData[name], value, state)
      this.UpdateCommand()
    },
    UpdateCommand() {
      console.log(JSON.stringify(this.formData))
    },
    //
    async SendMessage(operate) {
      if (util.isNullOrEmpty(operate.confirm)) {
        await util.operateHistory(this.history.id, operate.msg, operate.args)
      } else {
        util.confirmBox("警告", operate.confirm, async () => {
          await util.operateHistory(this.history.id, operate.msg, operate.args)
        })
      }
    },
    CheckCondition(condition) {
      if (condition == null) { return true }
      if (condition.servers != null && condition.servers.indexOf(util.serverName) < 0) return false
      if (condition.excludeServers != null && condition.excludeServers.indexOf(util.serverName) >= 0) return false
      if (condition.func != null && !(this[condition.func] || (()=>{}))()) return false
      return true
    },
    //==========================自定义函数==========================
    async OnClickUpdateEntry() {
      util.confirmEntry(this.history.platform, this.history.environment, this.history.unityVersion, async (entryName, updateEntryList) => {
        await util.operateHistory(this.history.id, HistoryOperate.updateEntry, { entry: entryName, updateEntryList: updateEntryList })
      })
    },
    async OnClickUpdateStageEntry() {
      util.confirmEntry(this.history.platform, "stage", this.history.unityVersion, async (entryName, updateEntryList) => {
        await util.operateHistory(this.history.id, HistoryOperate.updateEntry, { entry: entryName, updateEntryList: updateEntryList })
      })
    },
    async OnClickGenerateEnterIpa() {
      util.confirmBox("警告", "确定要生成企业签名包吗?", async () => {
        await util.operateHistory(this.history.id, HistoryOperate.generateEnterIpa)
      })
    },
    async OnClickUploadGooglePlay() {
      util.confirmBox("警告", "确定要上传apk obb到GooglePlay吗?", async () => {
        await util.operateHistory(this.history.id, HistoryOperate.uploadGooglePlay)
      })
    },
    async OnClickUploadApk() {
      util.confirmEntry(this.history.platform, this.history.environment, this.history.unityVersion, async (entryName) => {
        await util.operateHistory(this.history.id, HistoryOperate.uploadApk, { entry: entryName })
      })
    },


    IsAndroid() {
      return this.history.platform.toLowerCase() == "android"
    },
    IsIOS() {
      return this.history.platform.toLowerCase() == "ios"
    },
    IsGooglePC() {
      return this.history.environment != null && this.history.environment.indexOf("googlepc") >= 0
    },
    IsAmazonOrGooglePC() {
      return this.history.environment != null && (this.history.environment.indexOf("googlepc") >= 0 || this.history.environment.indexOf("amazon") >= 0)
    },
    IsNotGooglePC() {
      return !this.IsGooglePC()
    },
    IsNotAmazonOrGooglePC() {
      return !this.IsAmazonOrGooglePC()
    },
    IsUploadCDN() {
      return this.history.uploadCDN == true
    },
    IsProd() {
      return this.history.environment == "prod"
    },
  }
};
</script>
<style>
.history {
  font-size: 10px;
}
.history .ivu-form-item {
  margin-bottom: 0px;
}
</style>

