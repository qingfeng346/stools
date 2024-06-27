<template>
  <Layout>
    <Content>
      <img :src="actorInfo.imageUrl" :alt="actorInfo.name" style="height: 200px; width: 200px"/>
      <div>{{ actorInfo.name }}</div>
      <div>{{ actorInfo.desc }}</div>
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
