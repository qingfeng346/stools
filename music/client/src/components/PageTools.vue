<template>
  <Layout>
    <Content>
      <Button type="success" @click="OnClickRefresh">刷新列表</Button>
    </Content>
  </Layout>
</template>
<script>
import util from '@/scripts/util';

export default {
  methods: {
    OnClickRefresh() {
      util.UpdateMovieList()
    }
  },
};
</script>
