import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import ViewUIPlus from 'view-ui-plus'
import 'view-ui-plus/dist/styles/viewuiplus.css'
// import nativeUI from 'naive-ui'

const app = createApp(App)

app.use(router)
app.use(ViewUIPlus)
// app.use(nativeUI)

app.mount('#app')
