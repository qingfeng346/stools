<template>
    <Layout>
        <Layout>
        <!--
            <Sider>
                <Menu :active-name="activeMenu" theme="dark" width="auto" @on-select="OnSelectMenu">
                    <MenuItem name="music">音乐</MenuItem>
                </Menu>
            </Sider>
            -->
            <Layout>
                <Content>
                    <RouterView />
                </Content>
            </Layout>
        </Layout>
    </Layout>
    <a class="logButton" @click="OnClickLog"><Icon type="ios-chatboxes" style="margin:16px 16px;" size="32"/></a>
    <Modal v-model="showLog" fullscreen footer-hide title="日志">
      <div class="logParent">
        <pre id="viewLog">{{ logValue }}</pre>
      </div>
    </Modal>
</template>
<script>
import { Util } from 'weimingcommons'
import { RouterView } from 'vue-router'
import net from "../scripts/net";
import util from '../scripts/util'
export default {
    data() {
        return {
            activeMenu: "",
            showLog: false,
            logValue: "",
            logValueCache: "",
        }
    },
    beforeMount() {
        util.init(this.$Message, this.$Modal, this)
        this.UpdateMenu()
    },
    beforeUpdate() {
        this.UpdateMenu()
    },
    mounted() {
        this.viewLog = document.querySelector("#viewLog");
        net.registerMessage("write", this.OnMessage.bind(this));
        net.registerMessage("log", this.OnMessage.bind(this));
        net.registerMessage("notice", this.OnNotice.bind(this));
        this.UpdateScroll()
    },
    methods: {
        UpdateMenu() {
            let url = this.$route.fullPath;
            let index = url.lastIndexOf("/");
            let name = url.substring(index + 1);
            this.activeMenu = name;
        },
        OnSelectMenu(name) {
            if (name == this.activeMenu) {
                return;
            }
            this.$router.push(`${name}`);
        },
        async UpdateScroll() {
            while (true) {
                await Util.sleep(0.5)
                if (this.changed) {
                    this.changed = false
                    this.logValue = this.logValueCache
                    await Util.sleep(0.1)
                    this.viewLog.scrollTop = this.viewLog.scrollHeight;
                }
            }
        },
        OnMessage(data, code) {
            this.AddLog(data, code == "write")
        },
        AddLog(data, write) {
            if (write) {
                this.logValueCache += data;
            } else {
                this.logValueCache += `${data}\n`;
            }
            if (this.logValueCache.length > 81920) {
                this.logValueCache = this.logValueCache.substring(this.logValueCache.length - 81920);
            }
            this.changed = true
        },
        OnClickLog() {
            this.showLog = true
            this.changed = true
        },
        OnNotice(data) {
            if (data.type == "success") {
                util.noticeSuccess(data.msg)
            } else if (data.type == "error") {
                util.noticeError(data.msg)
            } else {
                util.noticeInfo(data.msg)
            }
        },
    }
}
</script>
<style>
.logParent {
  width: 100%;
  height: 100%;
  padding: 5px 0px 5px 0px;
}
.logButton {
  display: block;
  width: 64px;
  height: 64px;
  line-height: 64px;
  position: fixed;
  left: 32px;
  bottom: 32px;
  z-index: 999;
  background: #2d8cf0;
  color: #fff;
  border-radius: 6px;
  text-align: center;
  box-shadow: 0 2px 12px 0 rgb(0 0 0 / 40%);
}
pre {
  overflow: auto;
  white-space: pre-wrap;
  word-wrap: break-word;
  font-size: 13px;
  height: 100%;
  margin: 5px 5px;
  /* padding: 0; */
  line-height: 20px;
  /* overflow: scroll; */
}
</style>
