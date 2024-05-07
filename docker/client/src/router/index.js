import { createRouter, createWebHashHistory } from 'vue-router'
import home from '@/components/home.vue'
import music from '@/components/music.vue'
import photo from '@/components/photo.vue'
import PageBuild from '@/components/PageBuild.vue'
import PageHistory from '@/components/PageHistory.vue'
import PageConfig from '@/components/PageConfig.vue'
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
          redirect: '/home/build'
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
          name: "config",
          path: "config",
          component: PageConfig,
        },
        {
          name: "music",
          path: "music",
          component: music,
        },
        {
          name: "photo",
          path: "photo",
          component: photo,
        },
      ]
    }
  ]
})

export default router
