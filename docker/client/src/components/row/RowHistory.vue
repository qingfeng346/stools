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
                :divided="item.divided">
                {{ item.label }}
              </DropdownItem>
            </div>
          </DropdownMenu>
        </Dropdown>
      </FormItem>
      <div v-for="(info, index) in history.infos" :key="index">
        <Divider v-if="info.type == 1" size="small">{{ info.name }}</Divider>
        <FormItem v-else :label="info.name">
          <RowTime v-if="info.type == 2" :time="info.time" />
          <a v-else-if="info.type == 3" :href="info.link" target="_blank">{{ info.link }}</a>
          <span v-else-if="info.type == 4">
            <Input class="source" type="textarea" :rows="5" v-model="info.label" readonly />
          </span>
          <span v-else-if="info.type == 5">
            <Tag v-for="(value, key) in info.tags" :color="value.color" :key="key">{{ value.label }}</Tag>
          </span>
          <span v-else v-html="info.label"></span>
        </FormItem>
      </div>
    </Form>
  </Content>
</template>
<script>
import RowTime from "./RowTime.vue";
import util from "../../scripts/util";
export default {
  props: {
    history: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      operates: [],
      isShowMoreOperate: true,
    }
  },
  mounted() {
    this.Init()
  },
  methods: {
    async Init() {
      if (this.history.isFail) { return }
      // let commandInfo = (await util.GetCommandInfo(this.history.operate))?.operate
      // if (commandInfo == null) { return }
      this.operates = []
      // if (commandInfo.Operate != null) {
      //   let first = true
      //   for (let v of commandInfo.Operate) {
      //     let op = JSON.parse(JSON.stringify(v))
      //     if (!this.CheckCondition(op.condition)) {
      //       continue
      //     }
      //     if (!util.isNullOrEmpty(op.msg)) {
      //       op.type = 0
      //     } else if (!util.isNullOrEmpty(op.func)) {
      //       op.type = 1
      //       op.func = this[op.func].bind(this)
      //     }
      //     if (!util.isNullOrEmpty(op.vif)) {
      //       op.vif = this[op.vif].bind(this)
      //     } else {
      //       op.vif = this.GetTrue
      //     }
      //     if (first) {
      //       first = false
      //       op.divided = true
      //     }
      //     this.operates.push(op)
      //   }
      // }
      // if (commandInfo.OperateNew != null) {
      //   let first = true
      //   for (let v of commandInfo.OperateNew) {
      //     let op = JSON.parse(JSON.stringify(v))
      //     if (!this.CheckCondition(op.condition)) {
      //       continue
      //     }
      //     op.type = 2
      //     if (!util.isNullOrEmpty(op.vif)) {
      //       op.vif = this[op.vif].bind(this)
      //     } else {
      //       op.vif = this.GetTrue
      //     }
      //     if (first) {
      //       first = false
      //       op.divided = true
      //     }
      //     this.operates.push(op)
      //   }
      // }
      this.isShowMoreOperate = this.operates.length > 0
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

