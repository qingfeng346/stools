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
        <Table :columns="columns" :data="datas"></Table>
    </Layout>
</template>
<script>
import { Util, logger } from 'weimingcommons'
import net from '../scripts/net'
export default {
    data() {
        return {
            formItem: {},
            columns: [],
            datas: [],
            page: 1,
            pageSize: 50,
            total: 0,
        }
    },
    mounted() {
        this.columns = [
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
            },
            {
                title: "大小", key: "size"
            },
            {
                title: "下载时间", key: "time"
            }
        ]
        this.formItem.url = localStorage.getItem("url")
        this.UpdateMusicList()
    },
    methods: {
        async OnClickAlbum() {
            localStorage.setItem("url", this.formItem.url)
            await net.request("musicdownload", { type: "album", url : this.formItem.url })
            this.UpdateMusicList()
        },
        async OnClickMusic() {
            localStorage.setItem("url", this.formItem.url)
            await net.request("musicdownload", { type: "music", url : this.formItem.url })
            this.UpdateMusicList()
        },
        async UpdateMusicList() {
            let result = await net.request("musiclist", { page: this.page, pageSize: this.pageSize })
            this.total = result.total
            this.datas = result.datas
            for (let data of this.datas) {
                data.size = Util.getMemory(data.size)
                data.time = Util.formatDate(new Date(data.time))
            }
        }
    }
}
</script>