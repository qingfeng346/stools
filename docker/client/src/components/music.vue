<template>
    <Layout>
        <Button @click="OnClick">wfwefwefwef</Button>
    </Layout>
</template>
<script>
import net from '../scripts/net'
export default {
    methods: {
        async OnClick() {
            let result = await net.request("code", {aaa : 100, b: 200})
            console.log("==========================")
        }
    }
}
</script>