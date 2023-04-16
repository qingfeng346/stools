import { createRouter, createWebHistory } from 'vue-router'
import home from '@/components/home.vue'
import build from '@/components/build.vue'
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
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
          name: "build",
          path: "build",
          component: build,
        },
      ]
    }
  ]
})

export default router
