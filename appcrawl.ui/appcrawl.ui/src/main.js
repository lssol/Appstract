import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { Multipane, MultipaneResizer } from 'vue-multipane'

createApp(App).use(router).mount('#app')
