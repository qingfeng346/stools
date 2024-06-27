<template>
  <Layout>
    <Content>
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
      movies: []
    };
  },
  beforeMount() {
    this.type = util.getParam("type");
    this.value = util.getParam("value");
  },
  mounted() {
    this.RefreshAllInfos()
  },
  methods: {
    async RefreshAllInfos() {
      let infos = await util.GetMovieList(this.type, this.value)
      this.movies = infos
    },
  },
};
</script>
