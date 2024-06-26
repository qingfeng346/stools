import { createRouter, createWebHashHistory } from 'vue-router'
import home from '@/components/home.vue'
import PageBuild from '@/components/PageBuild.vue'
import PageHistory from '@/components/PageHistory.vue'
import PageList from '@/components/PageList.vue'
import PageConfig from '@/components/PageConfig.vue'
import PageTools from '@/components/PageTools.vue'
import InfoMovie from '@/components/InfoMovie.vue'
const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: '/',
      redirect: '/home'
    },
    {
      path: '/home',
      name: 'home',
      meta: [
        { charset: 'utf-8' },
      ],
      component: home,
      children: [
        {
          path: '',
          redirect: '/home/list'
        },
        {
          name: "build",
          path: "build",
          component: PageBuild,
        },
        {
          name: "history",
          path: "history",
          component: PageHistory,
        },
        {
          name: "list",
          path: "list",
          component: PageList,
        },
        {
          name: "config",
          path: "config",
          component: PageConfig,
        },
        {
          name: "tools",
          path: "tools",
          component: PageTools,
        },
        {
          name: "movieinfo",
          path: "movieinfo",
          component: InfoMovie,
        },
      ]
    }
  ]
})

export default router
