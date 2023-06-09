<template>
    <Layout style="minWidth:765px">
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
        <Page v-model="page" :total="total" :page-size="pageSize" show-total @on-change="UpdateMusicList" />
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
            pageSize: 100,
            total: 0,
        }
    },
    mounted() {
        this.columns = [
            {
                title: "名字", 
                key: "name",
                minWidth: 200,
            },
            {
                title: "专辑", 
                key: "album",
                minWidth: 150,
            },
            {
                title: "歌手", 
                key: "singer",
                width: 150
            },
            {
                title: "年份", 
                key: "year",
                width: 75
            },
            {
                title: "大小", 
                key: "size",
                width: 100
            },
            {
                title: "时长", 
                key: "duration",
                width: 90
            },
            // {
            //     title: "下载时间", key: "time"
            // },
            {
                title: "下载",
                width: 90,
                render: (h, params) => {
                    return h(
                        "a", {
                            href: params.row.downloadUrl,
                            target: "_blank",
                            style: {
                                color: "blue",
                                "text-decoration": "underline",
                            },
                        },
                        "链接"
                    );
                },
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
                data.duration = Util.getElapsedTimeString(data.duration)
                data.downloadUrl = `http://${window.location.hostname}:${window.location.port}/music/${data.singer}/${data.album}/${data.singer} - ${data.name}.mp3`
            }
        }
    }
}
</script>