import { createRouter, createWebHashHistory } from 'vue-router'
import home from '@/components/home.vue'
import PageAllMovies from '@/components/PageAllMovies.vue'
import PageConfig from '@/components/PageConfig.vue'
import PageTools from '@/components/PageTools.vue'
import InfoMovie from '@/components/InfoMovie.vue'
import InfoActor from '@/components/InfoActor.vue'
import InfoFilter from '@/components/InfoFilter.vue'

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
          redirect: '/home/allmovies'
        },
        {
          name: "allmovies",
          path: "allmovies",
          component: PageAllMovies,
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
          name: "movie",
          path: "movie",
          component: InfoMovie,
        },
        {
          name: "actor",
          path: "actor",
          component: InfoActor,
        },
        {
          name: "filter",
          path: "filter",
          component: InfoFilter,
        },
      ]
    }
  ]
})

export default router
