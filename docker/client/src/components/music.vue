<template>
    <Layout style="minWidth:765px">
        <Form :model="formItem" :label-width="80">
            <FormItem label="URL">
                <Input v-model="formItem.url" placeholder="url"></Input>
            </FormItem>
            <FormItem label="下载">
                <Space>
                    <Button type="primary" @click="OnClickRefresh">刷新</Button>
                    <Button type="primary" @click="OnClickCopy">复制并下载</Button>
                    <Button type="primary" @click="OnClickAlbum">下载专辑</Button>
                    <Button type="primary" @click="OnClickMusic">下载音乐</Button>
                </Space>
            </FormItem>
            <FormItem label="过滤">
                <Tag v-for="item in filter" :key="item.name" :name="item.name" type="border" closable color="primary" @on-close="OnClickRemoveFilter">{{ item.name }}</Tag>
                <Button icon="ios-add" type="dashed" size="small" @click="OnClickAddFilter">添加标签</Button>
            </FormItem>
        </Form>
        <Page v-model="page" :total="total" :page-size="pageSize" show-total @on-change="UpdateMusicList" />
        <Table :columns="columns" :data="datas">
            <template #action="{ row }">
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
            filter: []
        }
    },
    mounted() {
        this.columns = [
            {
                title: "名字", 
                key: "name",
                width: 200,
                sortable: true,
            },
            {
                title: "专辑", 
                key: "album",
                width: 200,
                sortable: true,
            },
            {
                title: "歌手", 
                key: "singer",
                width: 150,
                sortable: true,
            },
            {
                title: "年份", 
                key: "year",
                width: 85,
                sortable: true,
            },
            {
                title: "大小", 
                key: "size",
                width: 100,
                sortable: true,
            },
            {
                title: "时长", 
                key: "duration",
                width: 90,
                sortable: true,
            },
            {
                title: "路径", 
                key: "relative",
                minWidth: 350,
                sortable: true,
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
        async OnClickRefresh() {
            await this.UpdateMusicList()
        },
        async OnClickCopy() {
            this.formItem.url = await util.getClipboardText()
            await this.OnClickMusic()
        },
        async OnClickAlbum() {
            localStorage.setItem("url", this.formItem.url)
            await net.request("musicdownload", { type: "album", url : this.formItem.url })
            util.noticeInfo(`开始下载 : ${this.formItem.url}`)
            this.UpdateMusicList()
        },
        async OnClickMusic() {
            localStorage.setItem("url", this.formItem.url)
            await net.request("musicdownload", { type: "music", url : this.formItem.url })
            util.noticeInfo(`开始下载 : ${this.formItem.url}`)
            this.UpdateMusicList()
        },
        OnClickRemove(row) {
            util.confirmBox("警告",`确定要删除歌曲《${row.name}》吗？`, async () => {
                await net.request("musicdelete", { path: row.path })
                this.UpdateMusicList()
            })
        },
        async UpdateMusicList() {
            let result = await net.request("musiclist", { page: this.page, pageSize: this.pageSize, filter: this.filter })
            this.total = result.total
            this.datas = result.datas
            for (let data of this.datas) {
                data.size = Util.getMemory(data.size)
                data.time = Util.formatDate(new Date(data.time))
                data.duration = Util.getElapsedTimeString(data.duration)
                let index = data.path.indexOf("music")
                data.relative = data.path.substring(index + 6)
                data.downloadUrl = `http://${window.location.hostname}:${window.location.port}/music/${data.relative}`
            }
        },
        OnDownloadSuccess(data) {
            util.noticeSuccess(`下载成功 : ${data.path}`)
            this.UpdateMusicList()
        },
        OnClickRemoveFilter(evt, name) {
            let index = this.filter.findIndex((item) => { return item.name == name })
            if (index >= 0) {
                this.filter.splice(index, 1)
                this.UpdateMusicList()
            }
        },
        OnClickAddFilter() {
            util.confirmFilter((type, value) => {
                if (this.filter.findIndex((item) => { return item.type == type && item.value == value}) >= 0) { return }
                let filter = util.getFilter(type)
                this.filter.push({type: type, value: value, name: `${filter.label}:${value}`})
                this.UpdateMusicList()
            })
        }
    }
}
</script>