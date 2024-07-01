<template>
    <div class="content">
        <h1>{{ movieInfo.title }}</h1>
        <div style="text-align: center;">
            <img :src="movieInfo.imageUrl"/>
        </div>
        <Form label-position="left" :label-width="150">
            <FormItem label="文件路径">
                <div>{{ movieInfo.path }}</div>
            </FormItem>
            <FormItem label="简介">
                <div>{{ movieInfo.desc }}</div>
            </FormItem>
            <FormItem label="标签">
                <Space wrap>
                    <Button v-for="tag of movieInfo.tags" size="large" type="text" @click="OnClickTag(tag)">{{ tag }}</Button>
                </Space>
            </FormItem>
            <FormItem label="制作">
                <Space wrap>
                    <Button v-for="maker of movieInfo.makers" size="large" type="text" @click="OnClickMaker(maker)">{{ maker }}</Button>
                </Space>
            </FormItem>
            <FormItem label="发行">
                <Space wrap>
                    <Button v-for="label of movieInfo.labels" size="large" type="text" @click="OnClickLabel(label)">{{ label }}</Button>
                </Space>
            </FormItem>
            <FormItem label="系列">
                <Space wrap>
                    <Button v-for="serie of movieInfo.series" size="large" type="text" @click="OnClickSerie(serie)">{{ serie }}</Button>
                </Space>
            </FormItem>
            <FormItem label="操作">
                <Space wrap>
                    <Button size="large" type="text" @click="OnClickPlay()">播放地址</Button>
                    <Button size="large" type="text" @click="OnClickUpdateInfo()">刷新数据</Button>
                </Space>
            </FormItem>
        </Form>
        <h2>演员</h2>
        <Row :gutter="20">
          <Col v-for="actorInfo in movieInfo.actors" :key="actorInfo.id">
            <Card :hoverable="true">
              <img :src="actorInfo.imageUrl" :alt="actorInfo.name" style="height: 150px; width: 150px" @click="OnClickActor(actorInfo)"/>
              <Ellipsis :text="actorInfo.name" :length="12" tooltip />
            </Card>
          </Col>
        </Row>
        <h2>截图</h2>
        <Space wrap>
            <template v-for="(url,index) in movieInfo.shotscreens" :key="url">
                <Image :src="url" fit="contain" width="120px" height="80px" preview :preview-list="movieInfo.shotscreens" :initial-index="index" />
            </template>
        </Space>
        <Form label-position="left" :label-width="150">
            <FormItem label="解析类型">
                <Input v-model="parseType" placeholder="解析类型" />
            </FormItem>
            <FormItem label="解析内容">
                <Input v-model="parseContent" type="textarea" :rows="10" placeholder="解析内容" />  
            </FormItem>
            <FormItem label="解析内容">
                <Button @click="OnClickParse">开始解析</Button>
            </FormItem>
        </Form>
    </div>
</template>
<script>
import util from "../scripts/util";
import net from "../scripts/net";
export default {
    data() {
        return {
            movieInfo : {},
            parseType: "",
            parseContent: ""
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
            if (movieInfo.actors != null) {
                for (let actorId of movieInfo.actors) {
                    actors.push(await util.GetPersonInfo(actorId))
                }
            }
            movieInfo.actors = actors;
            movieInfo.tags = movieInfo.tags ?? []
            movieInfo.makers = movieInfo.makers ?? []
            movieInfo.genres = movieInfo.genres ?? []
            movieInfo.series = movieInfo.series ?? []
            movieInfo.shotscreens = movieInfo.shotscreens ?? []
            this.movieInfo = movieInfo
        },
        async OnClickPlay() {
            window.open(`${net.ServerUrl}/assets/media/${this.movieInfo.path}`, "_blank")
        },
        async OnClickUpdateInfo() {
          util.UpdateMoveInfo(this.id)
        },
        async OnClickActor(actor) {
            this.$router.push(`/home/actor?id=${actor.id}`);
        },
        async OnClickTag(value) {
            this.$router.push(`/home/filter?type=tag&value=${value}`);
        },
        async OnClickMaker(value) {
            this.$router.push(`/home/filter?type=maker&value=${value}`);
        },
        async OnClickLabel(value) {
            this.$router.push(`/home/filter?type=label&value=${value}`);
        },
        async OnClickSerie(value) {
            this.$router.push(`/home/filter?type=serie&value=${value}`);
        },
        async OnClickParse() {
          util.ParseMoveInfo(this.id, this.parseType, this.parseContent)
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
  