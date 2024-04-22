import { createRouter, createWebHashHistory, createWebHistory } from 'vue-router'
import home from '@/components/home.vue'
import music from '@/components/music.vue'
import photo from '@/components/photo.vue'
import PageBuild from '@/components/PageBuild.vue'
const router = createRouter({
  // history: createWebHistory(import.meta.env.BASE_URL),
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
