<template>
  <Layout>
    <Content>
    <div class="movie-wall">
      <Row :gutter="20">
        <Col v-for="movie in movies" :key="movie.id">
          <Card :hoverable="true">
            <img :src="movie.poster" :alt="movie.title" style="height: 200px; width: 150px" />
            <h3>{{ movie.title }}</h3>
          </Card>
        </Col>
      </Row>
      </div>
      <List item-layout="vertical">
        <ListItem v-for="item in datas" :key="item.title">
          <RowList :movieInfo="item" />
        </ListItem>
      </List>
    </Content>
  </Layout>
</template>
<script>
import RowList from "./row/RowList.vue";
import net from "../scripts/net";
import util from "../scripts/util";
import code from '../scripts/code.js';
const { RequestCode, Status } = code;
import { resolveComponent } from 'vue';

export default {
  components : {
    RowList,
  },
  data() {
    return {
      movies: [
        { id: 1, title: 'Movie 1', poster: 'https://via.placeholder.com/150' },
        { id: 2, title: 'Movie 2', poster: 'https://via.placeholder.com/150' },
        { id: 3, title: 'Movie 3', poster: 'https://via.placeholder.com/150' },
        { id: 4, title: 'Movie 4', poster: 'https://via.placeholder.com/150' },
        { id: 5, title: 'Movie 5', poster: 'https://dev-file.iviewui.com/5wxHCQMUyrauMCGSVEYVxHR5JmvS7DpH/large' },
        { id: 6, title: 'Movie 6', poster: 'https://via.placeholder.com/150' },
        { id: 7, title: 'Movie 7', poster: 'https://via.placeholder.com/150' },
        { id: 8, title: 'Movie 8', poster: 'https://via.placeholder.com/150' },
      ]
    };
  },
  beforeMount() {
    this.datas = [
      {
        title : "111",
        avatar: "https://dev-file.iviewui.com/5wxHCQMUyrauMCGSVEYVxHR5JmvS7DpH/large",
        description:"desc", 
        content: "content"
      },
      {
        title : "2222",
        avatar: "https://dev-file.iviewui.com/5wxHCQMUyrauMCGSVEYVxHR5JmvS7DpH/large",
        description:"desc", 
        content: "content"
      }
    ]
  },
  mounted() {
    // util.registerOnResize(this, [{ name: "tableHeight", value: 0 }])
    // this.RefreshTable()
  },
  methods: {
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
