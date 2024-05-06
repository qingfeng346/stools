<template>
  <Content class="content">
    <Tabs>
      <TabPane name="config" label="配置">
        <Select v-model="selectedConfigName" :disabled="loading" style="width: 250px" @on-select="OnSelectConfig">
          <Option v-for="configName in configNames" :key="configName" :value="configName">{{ configName }}</Option>
        </Select>
        <Input v-model="currentConfigName" :disabled="loading" placeholder="" style="width: 250px" />
        <Button type="primary" :disabled="loading" @click="OnClickGetConfig">请求</Button>
        <Button type="primary" :disabled="loading" @click="OnClickSetConfig">保存</Button>
        <Button type="error" :disabled="loading"   @click="OnClickDelConfig">删除</Button>
        <Input class="source" :disabled="loading" v-model="configContent" type="textarea" :rows="30" @on-blur="OnFormatConfigContent"/>
      </TabPane>
      <TabPane name="command" label="命令">
        <Select v-model="selectedCommandName" :disabled="loading" style="width: 250px" @on-select="OnSelectCommand">
          <Option v-for="commandName in commandNames" :key="commandName" :value="commandName">{{ commandName }}</Option>
        </Select>
        <Input v-model="currentCommandName" :disabled="loading" placeholder="" style="width: 250px" />
        <Button type="primary" :disabled="loading" @click="OnClickGetCommand">请求</Button>
        <Button type="primary" :disabled="loading" @click="OnClickSetCommand">保存</Button>
        <Button type="error" :disabled="loading"   @click="OnClickDelCommand">删除</Button>
        <Collapse accordion v-model="commandCollapse">
          <Panel name="1">
            信息
            <template #content>
              <Input class="source" :disabled="loading" v-model="commandInfo.info" type="textarea" :rows="20" @on-blur="OnFormatCommandInfo"/>
            </template>
          </Panel>
          <Panel name="2">
            命令
            <template #content>
              <Input class="source" :disabled="loading" v-model="commandInfo.content" type="textarea" :rows="20" @on-blur="OnFormatCommandInfo"/>
            </template>
          </Panel>
          <Panel name="3">
            执行
            <template #content>
              <Input class="source" :disabled="loading" v-model="commandInfo.execute" type="textarea" :rows="20" @on-blur="OnFormatCommandInfo"/>
            </template>
          </Panel>
          <Panel name="4">
            后续操作
            <template #content>
              <Input class="source" :disabled="loading" v-model="commandInfo.operate" type="textarea" :rows="20" @on-blur="OnFormatCommandInfo"/>
            </template>
          </Panel>
        </Collapse>
      </TabPane>
    </Tabs>
  </Content>
</template>
<script>
import code from '../scripts/code.js';
const { RequestCode, ConfigType } = code;
import net from '../scripts/net';
import util from '../scripts/util';
const StorageSelectedConfig = "PageConfig_StorageSelectedConfig"
const StorageSelectedCommand = "PageConfig_StorageSelectedCommand"
export default {
  data() {
    return {
      loading: false,

      configNames: [],
      selectedConfigName: "",
      currentConfigName: "",
      configContent: "",

      
      commandNames: [],
      selectedCommandName: "",
      currentCommandName: "",
      commandInfo : {
        info: "",
        content: "",
        execute: "",
        operate: "",
      },
      commandCollapse: "1",
    };
  },
  mounted() {
    this.selectedConfigName = localStorage.getItem(StorageSelectedConfig);
    this.currentConfigName = this.selectedConfigName
    this.UpdateConfigList()
    this.OnClickGetConfig()

    this.selectedCommandName = localStorage.getItem(StorageSelectedCommand);
    this.currentCommandName = this.selectedCommandName
    this.UpdateCommandList()
    this.OnClickGetCommand()
  },
  methods: {
    async UpdateConfigList() {
      this.configNames = []
      for (let v of await net.execute(RequestCode.GetConfigList)) {
        this.configNames.push(v.name)
      }
      if (this.configNames.indexOf(this.selectedConfigName) < 0) {
        this.selectedConfigName = this.configNames[0]
      }
      if (this.configNames.indexOf(this.currentConfigName) < 0) {
        this.currentConfigName = this.configNames[0]
        await this.OnClickGetConfig()
      }
    },
    async OnSelectConfig(select) {
      this.currentConfigName = select.value
      await this.OnClickGetConfig()
    },
    async OnClickGetConfig() {
      if (util.isNullOrEmpty(this.currentConfigName)) { return }
      localStorage.setItem(StorageSelectedConfig, this.currentConfigName);
      this.loading = true
      let result = await util.GetConfig(this.currentConfigName, true)
      this.loading = false
      this.configContent = JSON.stringify(result, null, 2)
      await this.UpdateConfigList()
    },
    async OnClickSetConfig() {
      if (util.isNullOrEmpty(this.currentConfigName)) { return }
      this.loading = true
      if (!await util.SetConfig(this.currentConfigName, this.configContent)) {
        this.loading = false
        return;
      }
      await this.UpdateConfigList()
      this.loading = false
      util.successMessage("保存成功")
    },
    OnClickDelConfig() {
      if (util.isNullOrEmpty(this.currentConfigName)) { return }
      util.confirmBox("警告", `确定要删除配置[${this.currentConfigName}]吗?<br>此操作不可逆转!!!`, async () => {
        await util.DelConfig(this.currentConfigName)
        await this.UpdateConfigList()
      }, true);
    },
    OnFormatConfigContent() {
      if (util.isNullOrEmpty(this.currentConfigName)) { return }
      try {
        this.configContent = util.formatJson(this.configContent)
      } catch (e) {
        util.errorMessage(`内容不是正确的JSON格式:${e}`);
      }
    },


    async UpdateCommandList() {
      this.commandNames = []
      for (let v of await util.GetCommandList()) {
        this.commandNames.push(v.name)
      }
      if (this.commandNames.indexOf(this.selectedCommandName) < 0) {
        this.selectedCommandName = this.commandNames[0]
      }
      if (this.commandNames.indexOf(this.currentCommandName) < 0) {
        this.currentCommandName = this.commandNames[0]
        await this.OnClickGetCommand()
      }
    },
    async OnSelectCommand(select) {
      this.currentCommandName = select.value
      await this.OnClickGetCommand()
    },
    async OnClickGetCommand() {
      if (util.isNullOrEmpty(this.currentCommandName)) { return }
      localStorage.setItem(StorageSelectedCommand, this.currentCommandName);
      this.loading = true
      let commandInfo = await util.GetCommand(this.currentCommandName, true)
      this.loading = false
      if (commandInfo == null) {
        this.commandInfo = {
          info : "{}",
          content : "{}",
          execute : "{}",
          operate : "{}",
        }
      } else {
        this.commandInfo = {
          info : JSON.stringify(commandInfo.info),
          content : JSON.stringify(commandInfo.content),
          execute : JSON.stringify(commandInfo.execute),
          operate : JSON.stringify(commandInfo.operate),
        }
      }
      this.OnFormatCommandInfo()
    },
    async OnClickSetCommand() {
      if (util.isNullOrEmpty(this.currentCommandName)) { return }
      this.loading = true
      if (!await util.SetCommand(this.currentCommandName, this.commandInfo)) {
        this.loading = false
        return;
      }
      util.successMessage("保存成功")
      await this.UpdateCommandList()
      this.loading = false
    },
    OnClickDelCommand() {
      if (util.isNullOrEmpty(this.currentCommandName)) { return }
      util.confirmBox("警告", `确定要删除命令[${this.currentCommandName}]吗?<br>此操作不可逆转!!!`, async () => {
        await util.DelCommand(this.currentCommandName)
        await this.UpdateCommandList()
      })
    },
    OnFormatCommandInfo() {
      if (util.isNullOrEmpty(this.currentCommandName)) { return }
      try {
        this.commandInfo.info = util.formatJson(this.commandInfo.info)
        this.commandInfo.content = util.formatJson(this.commandInfo.content)
        this.commandInfo.execute = util.formatJson(this.commandInfo.execute)
        this.commandInfo.operate = util.formatJson(this.commandInfo.operate)
      } catch (e) {
        util.errorMessage(`内容不是正确的JSON格式:${e}`);
      }
    },
  },
};
</script>