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
import net from "../scripts/net";
export default {
  components : {
    TemplateMovieList,
  },
  data() {
    return {
      movies: []
    };
  },
  mounted() {
    this.RefreshList()
  },
  methods: {
    async RefreshList() {
      this.movies = await util.GetMovieList() ?? []
    },
  },
};
</script>
