<template>
    <Layout>
        <Form :model="formItem" :label-width="80">
            <FormItem label="URL">
                <Input v-model="formItem.url" placeholder="url"></Input>
            </FormItem>
            <FormItem label="下载">
                <Space>
                    <Button type="primary" @click="OnClickAlbum">下载专辑</Button>
                    <Button type="primary" @click="OnClickMusic">下载音乐</Button>
                </Space>
            </FormItem>
        </Form>
        <Page v-model="page" :total="total" :page-size="pageSize" show-total />
        <Table :columns="columns" :data="data"></Table>
    </Layout>
</template>
<script>
import net from '../scripts/net'
export default {
    data() {
        return {
            formItem: {},
            columns: [],
            data: [],
            page: 1,
            pageSize: 50,
            total: 0,
        }
    },
    mounted() {
        this.columns = [
            {
                title: "ID", key: "musicId"
            },
            {
                title: "平台", key: "musicType"
            },
            {
                title: "名字", key: "name"
            },
            {
                title: "专辑", key: "album"
            },
            {
                title: "歌手", key: "singer"
            },
            {
                title: "发型年份", key: "year"
            }
        ]
        this.UpdateMusicList()
    },
    methods: {
        async OnClickAlbum() {
            let result = await net.request("musicdownload", { type: "album", url : this.formItem.url })
        },
        async OnClickMusic() {
            let result = await net.request("musicdownload", { type: "music", url : this.formItem.url })
        },
        async UpdateMusicList() {
            let result = await net.request("musiclist", { page: this.page, pageSize: this.pageSize })
            this.total = result.total
            this.data = result.datas
        }
    }
}
</script>