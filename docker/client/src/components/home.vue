<template>
    <Layout>
        <Layout>
            <Sider>
                <Menu :active-name="activeMenu" theme="dark" width="auto" @on-select="OnSelectMenu">
                    <MenuItem name="music">音乐</MenuItem>
                </Menu>
            </Sider>
            <Layout>
                <Content>
                    <RouterView />
                </Content>
            </Layout>
        </Layout>
    </Layout>
</template>
<script>
import { RouterView } from 'vue-router'
export default {
    data() {
        return {
            activeMenu: ""
        }
    },
    beforeMount() {
        this.UpdateMenu()
    },
    beforeUpdate() {
        this.UpdateMenu()
    },
    methods: {
        UpdateMenu() {
            let url = this.$route.fullPath;
            let index = url.lastIndexOf("/");
            let name = url.substring(index + 1);
            this.activeMenu = name;
        },
        OnSelectMenu(name) {
            if (name == this.activeMenu) {
                return;
            }
            this.$router.push(`${name}`);
        }
    }
}
</script>
