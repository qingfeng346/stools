<template>
  <Layout>
    <Content>
      <div class="movie-wall">
        <Row :gutter="20">
          <Col v-for="movie in movies" :key="movie.id">
            <Card :hoverable="true">
              <img :src="movie.thumbUrl" :alt="movie.title" style="height: 240px; width: 180px" @click="OnClickMovie(movie)" />
              <Ellipsis :text="movie.title" :length="12" tooltip />
            </Card>
          </Col>
        </Row>
      </div>
    </Content>
  </Layout>
</template>
<script>
import RowList from "./row/RowList.vue";
import util from "../scripts/util";
export default {
  components : {
    RowList,
  },
  data() {
    return {
      movies: []
    };
  },
  beforeMount() {
  },
  mounted() {
    this.RefreshAllInfos()
  },
  methods: {
    async RefreshAllInfos() {
      let infos = await util.GetMovieList()
      this.movies = infos
    },
    OnClickMovie(movie) {
      this.$router.push(`/home/movieinfo?id=${movie.id}`);
      console.log(movie)
    }
  },
};
</script>
<style>
.movie-wall {
  padding: 20px;
}
.movie-wall h3 {
  text-align: center;
  margin-top: 10px;
}
</style>
