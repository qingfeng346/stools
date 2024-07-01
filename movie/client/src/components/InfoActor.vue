<template>
  <Layout>
    <Content>
      <h2>{{ actorInfo.name }}</h2>
      <div style="text-align: center;">
        <img :src="actorInfo.imageUrl" :alt="actorInfo.name" class="Image"/>
      </div>
      <Form label-position="left" :label-width="150">
        <FormItem label="简介">
          <pre>{{ actorInfo.desc }}</pre>
        </FormItem>
        <FormItem label="操作">
          <Space wrap>
            <Button size="large" type="success" ghost @click="OnClickUpdateInfo()">刷新数据</Button>
            <Button size="large" type="success" ghost @click="OnClickUpdateServerInfo()">刷新服务器数据</Button>
          </Space>
        </FormItem>
        <FormItem label="解析类型">
          <AutoComplete
              v-model="parseType"
              :data="parseTypeList"
              placeholder="解析类型">
          </AutoComplete>
        </FormItem>
        <FormItem label="解析内容">
            <Input v-model="parseContent" type="textarea" :rows="10" placeholder="解析内容" />  
        </FormItem>
        <FormItem label="解析内容">
            <Button type="success" ghost @click="OnClickParse">解析数据</Button>
        </FormItem>
      </Form>
      <TemplateMovieList :movies="movies"/>
    </Content>
  </Layout>
</template>
<script>
import TemplateMovieList from "./template/TemplateMovieList.vue";
import util from "../scripts/util";
export default {
  components : {
    TemplateMovieList,
  },
  data() {
    return {
      actorInfo: {},
      movies: [],
      parseType: "",
      parseTypeList: ["ProviderTest", "ProviderTest2", "ProviderTest3"],
      parseContent: ""
    };
  },
  beforeMount() {
    this.id = util.getParam("id");
  },
  mounted() {
    this.RefreshInfo()
  },
  methods: {
    async RefreshInfo() {
      this.actorInfo = await util.GetPersonInfo(this.id)
      let infos = await util.GetMovieList("actor", this.id)
      this.movies = infos
    },
    async OnClickUpdateInfo() {
      await this.RefreshInfo()
      util.noticeSuccess("刷新成功")
    },
    async OnClickUpdateServerInfo() {
      util.UpdatePersonInfo(this.id)
    },
    async OnClickParse() {
      util.ParsePersonInfo(this.id, this.parseType, this.parseContent)
    }
  },
};
</script>
<style>
h2 {
  text-align: center;
  margin-top: 10px;
}
.Image {
  height: 200px;
  width: 200px;
}
</style>