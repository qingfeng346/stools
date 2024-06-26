<template>
  <Content class="history">
    <Row>
        <Col>
          <img :src="movieInfo.avatar" style="maxWidth:250px">
        </Col>
        <Col>
        <div>123123123</div>
        <div>123123123</div>
        <div>123123123</div>
        <div>123123123</div>
        <div>123123123</div>
        <div>123123123</div>
        <div>123123123</div>
        </Col>
    </Row>
    <Form label-position="left" :label-width="150">
      <FormItem label="任务ID">
        <span> {{ movieInfo.title }} </span>
      </FormItem>
    </Form>
  </Content>
</template>
<script>
import util from "../../scripts/util";
export default {
  props: {
    movieInfo: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
    }
  },
  mounted() {
    this.Init()
  },
  methods: {
    async Init() {
    }
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

