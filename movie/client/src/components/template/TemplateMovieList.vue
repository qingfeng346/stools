<template>
  <Content>
    <div class="movie-wall">
      <Row :gutter="20">
        <Col v-for="movie in movies" :key="movie.id">
          <Card :hoverable="true">
            <img :src="GetThumbUrl(movie)" :alt="movie.title" style="height: 240px; width: 180px" @click="OnClickMovie(movie)" />
            <Ellipsis :text="movie.title ?? movie.path" :length="22" tooltip style="text-align: center;" />
          </Card>
        </Col>
      </Row>
    </div>
  </Content>
</template>
<script>
import util from "../../scripts/util";
import net from "../../scripts/net";
export default {
  props: {
    movies: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
    };
  },
  mounted() {
  },
  methods: {
    OnClickMovie(movie) {
      this.$router.push(`/home/movie?id=${movie.id}`);
    },
    GetThumbUrl(movie) {
      return movie.thumbUrl ?? `${net.ServerUrl}/assets/images/180x240.png`
    }
  },
};
</script>
<style>
.movie-wall {
  padding: 20px;
}
</style>
