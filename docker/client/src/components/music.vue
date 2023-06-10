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
        <Table :columns="columns" :data="datas">
            <template #action="{ row, index }">
                <a :href="row.downloadUrl" target="_blank" style="margin-right: 10px;">下载</a>
                <Button type="error" size="small" @click="OnClickRemove(row)">删除</Button>
            </template>
        </Table>
    </Layout>
</template>
<script>
import { Util } from 'weimingcommons'
import net from '../scripts/net'
import util from '../scripts/util'
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
            {
                title: "操作",
                width: 120,
                slot: 'action',
            }
        ]
        this.formItem.url = localStorage.getItem("url")
        this.UpdateMusicList()
        net.registerMessage("downloadSuccess", this.OnDownloadSuccess.bind(this));
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
        OnClickRemove(row) {
            util.confirmBox("警告",`确定要删除歌曲《${row.name}》吗？`, async () => {
                await net.request("musicdelete", { path: row.path })
                this.UpdateMusicList()
            })
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
        },
        OnDownloadSuccess(data) {
            util.noticeSuccess(`下载成功 : ${data.path}`)
            this.UpdateMusicList()
        }
    }
}
</script>