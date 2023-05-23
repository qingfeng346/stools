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
        <Page v-model="page" :total="100" page-size="20" show-total />
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
            page: 1
        }
    },
    methods: {
        async OnClickAlbum() {
            let result = await net.request("album", { url : this.formItem.url })
        },
        async OnClickMusic() {
            let result = await net.request("music", { url : this.formItem.url })
        },
        async UpdateMusic() {

        }
    }
}
</script>