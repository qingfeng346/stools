<template>
  <Content>
    <Page class="page"
          :current="0"
          :total="GetPageSize()"
          :page-size="GetPageSize()"
          show-total
          show-elevator
          @on-change="OnChangePage"
        />
    <Input search v-model="searchValue" placeholder="Enter something..." @on-search="OnSearchChanged"/>
    <div class="movie-wall">
      <Row :gutter="20">
        <Col v-for="movie in showMovies" :key="movie.id">
          <Card :hoverable="true">
            <img :src="GetThumbUrl(movie)" :alt="movie.title" style="height: 240px; width: 180px" @click="OnClickMovie(movie)" />
            <Ellipsis :text="movie.title ?? movie.path" :length="11" tooltip style="text-align: center;" />
          </Card>
        </Col>
      </Row>
    </div>
  </Content>
</template>
<script>
import util from "../../scripts/util";
export default {
  props: {
    movies: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      searchValue: "",
      showMovies: [],
    };
  },
  watch: {
    movies: function(oldValue, newValue) {
      this.UpdateMovies()
    }
  },
  methods: {
    OnClickMovie(movie) {
      this.$router.push(`/home/movie?id=${movie.id}`);
    },
    GetThumbUrl(movie) {
      return movie.thumbUrl ?? util.GetDefaultImage150x200()
    },
    GetPageSize() {
      return this.movies.length
    },
    UpdateMovies() {
      this.OnSearchChanged()
    },
    OnSearchChanged() {
      console.log("searchchanged " + this.searchValue)
      if (util.isNullOrEmpty(this.searchValue)) {
        this.showMovies = this.movies
      } else {
        this.showMovies = []
        let value = this.searchValue
        for (let movie of this.movies) {
          if (movie.path.indexOf(value) >= 0 ||
              movie.title.indexOf(value) >= 0) {
              this.showMovies.push(movie)
          }
        }
      }
    },
    OnChangePage() {

    }
  },
};
</script>
<style>
.movie-wall {
  padding: 20px;
}
</style>
