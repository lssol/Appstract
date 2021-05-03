import Vue from 'vue'
import VueRouter from 'vue-router'
import Application from '../views/Application.vue'
import Home from '../views/Home.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/application/:id?',
    name: 'Application',
    component: Application
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
