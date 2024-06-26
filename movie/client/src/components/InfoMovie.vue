<template>
    <div class="content">
        <h1>{{ movieInfo.title }}</h1>
        <img :src="movieInfo.imageUrl"/>
        <Form label-position="left" :label-width="150">
            <FormItem label="标签">
                <Space wrap>
                    <Button v-for="tag of movieInfo.tags" :size="buttonSize" type="text">{{ tag }}</Button>
                </Space>
            </FormItem>
        </Form>
        <h2>演职人员</h2>
        <Row :gutter="20">
          <Col v-for="actor in movieInfo.actors" :key="actor.id">
            <Card :hoverable="true">
              <img :src="actor.imageUrl" :alt="actor.name" style="height: 150px; width: 150px" />
              <Ellipsis :text="actor.name" :length="12" tooltip />
            </Card>
          </Col>
        </Row>
        <h2>截图</h2>
        <Space wrap>
            <template v-for="(url,index) in movieInfo.shotscreens" :key="url">
                <Image :src="url" fit="contain" width="120px" height="80px" preview :preview-list="movieInfo.shotscreens" :initial-index="index" />
            </template>
        </Space>
    </div>
</template>
<script>
import util from "../scripts/util";
import net from "../scripts/net";
export default {
    data() {
        return {
            movieInfo : {}
        };
    },
    beforeMount() {
        this.id = util.getParam("id");
    },
    mounted() {
        this.RefreshInfo();
    },
    methods: {
        async RefreshInfo() {
            let movieInfo = await util.GetMovieInfo(this.id);
            let actors = []
            for (let actor of movieInfo.actors) {
                console.log(actor)
                actors.push(await util.GetPersonInfo(actor))
            }
            movieInfo.actors = actors;
            this.movieInfo = movieInfo
        }
    },
};
</script>
<style>
h1 {
  text-align: center;
  margin-top: 10px;
}
</style>
  