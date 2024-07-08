<template>
  <div class="layout">
    <Layout style="height: 100%; width: 100%">
      <Header style="margin:0px; padding:0px 10px;height:64px">
        <div style="float:left;display:inline">
          <a @click="OnClickCollapsedMenu"><Icon :class="rotateIcon" style="margin:16px 16px" type="md-menu" size="32"/></a>
          <span class="title">工具</span>
        </div>
      </Header>
      <Layout style="width: 100%; height: calc(100% - 64px);">
        <Sider v-if="showMenu">
          <Menu :active-name="activeMenu" theme="dark" width="auto" @on-select="OnSelectMenu">
            <MenuItem name="allmovies">列表</MenuItem>
            <MenuItem name="tools">工具</MenuItem>
          </Menu>
        </Sider>
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
  </div>
</template>
<script>
import { Util } from 'weimingcommons'
import { RouterView } from 'vue-router'
import net from "../scripts/net";
import util from '../scripts/util'
export default {
  data() {
    return {
      showMenu: true,
      activeMenu: "",
      showLog: false,
      logValue: "",
      logValueCache: "",
    };
  },
  beforeMount() {
    util.init(this.$Message, this.$Modal, this);
    this.UpdateMenu();
  },
  beforeUpdate() {
    this.UpdateMenu();
  },
  mounted() {
    this.viewLog = document.querySelector("#viewLog");
    net.registerMessage("write", this.OnMessage.bind(this));
    net.registerMessage("log", this.OnMessage.bind(this));
    net.registerMessage("notice", this.OnNotice.bind(this));
    this.UpdateScroll();
  },
  computed: {
    rotateIcon() {
      return ["menu-icon", this.showMenu ? "" : "rotate-icon"];
    },
  },
  methods: {
    UpdateMenu() {
      let url = this.$route.fullPath;
      let index = url.lastIndexOf("/");
      let name = url.substring(index + 1);
      this.activeMenu = name;
    },
    OnClickCollapsedMenu() {
      this.showMenu = !this.showMenu;
    },
    OnSelectMenu(name) {
      if (name == this.activeMenu) {
        return;
      }
      this.$router.push(`${name}`);
    },
    async UpdateScroll() {
      while (true) {
        await Util.sleep(0.5);
        if (this.changed) {
          this.changed = false;
          this.logValue = this.logValueCache;
          await Util.sleep(0.1);
          this.viewLog.scrollTop = this.viewLog.scrollHeight;
        }
      }
    },
    OnMessage(data, code) {
      this.AddLog(data, code == "write");
    },
    AddLog(data, write) {
      if (write) {
        this.logValueCache += data;
      } else {
        this.logValueCache += `${data}\n`;
      }
      if (this.logValueCache.length > 1048576) {
        this.logValueCache = this.logValueCache.substring(this.logValueCache.length - 1048576);
      }
      this.changed = true;
    },
    OnClickLog() {
      this.showLog = true;
      this.changed = true;
    },
    OnNotice(data) {
      if (data.type == "success") {
        util.noticeSuccess(data.msg);
      } else if (data.type == "error") {
        util.noticeError(data.msg);
      } else {
        util.noticeInfo(data.msg);
      }
    }
  }
}
</script>
<style>
.layout .ivu-form-item {
  margin-bottom: 10px;
}
.layout{
  border: 1px solid #d7dde4;
  background: #f5f7f9;
  position: relative;
  border-radius: 4px;
  overflow: hidden;
}
.menu-icon {
  color: white;
  transition: all 0.3s;
}
.rotate-icon {
  transform: rotate(-90deg);
}
.title {
  text-align: center;
  font-size: 18px;
  color: #fff;
}
.content {
  margin: 20px;
}
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
