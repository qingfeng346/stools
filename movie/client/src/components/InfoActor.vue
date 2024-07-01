<template>
  <Layout>
    <Content>
      <h1>{{ actorInfo.name }}</h1>
      <div style="text-align: center;">
        <img :src="actorInfo.imageUrl" :alt="actorInfo.name" class="Image"/>
      </div>
      <div>{{ actorInfo.desc }}</div>
      <Button size="large" type="text" @click="OnClickUpdateInfo()">刷新数据</Button>
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
    };
  },
  beforeMount() {
    this.id = util.getParam("id");
  },
  mounted() {
    this.RefreshAllInfos()
  },
  methods: {
    async RefreshAllInfos() {
      this.actorInfo = await util.GetPersonInfo(this.id)
      let infos = await util.GetMovieList("actor", this.id)
      this.movies = infos
    },
    async OnClickUpdateInfo() {
      util.UpdatePersonInfo(this.id)
    },
  },
};
</script>
<style>
h1 {
  text-align: center;
  margin-top: 10px;
}
.Image {
  height: 200px;
  width: 200px;
}
</style>