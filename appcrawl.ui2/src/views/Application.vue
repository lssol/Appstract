<template>
  <div :style="{height: '100vh'}">

    <div id="title" :style="{height: '60px'}">
      <h4>
        <label-edit v-bind:text="application.name" v-on:text-updated-blur="updateName" v-on:text-updated-enter="updateName"></label-edit>
      </h4>
    </div>

    <div class="columns full-height">
      <div class="full-height column is-one-fifth">
        <template-selector v-model:template="template" :templates="application.templates"/>
      </div>
      <div class="full-height full-width">
        <span>{{template}}</span>
        <label-edit v-bind:text="url" v-on:text-updated-blur="updateUrl" v-on:text-updated-enter="updateUrl"></label-edit>
        <template-view v-bind:url="url"/>
      </div>
    </div>

  </div>
</template>

<style scoped>
  #title {
    padding-left: 10px;
    padding-top: 10px;
  } 
  #title h4 {
    width: fit-content
  }
</style>

<script>
import TemplateSelector from '@/components/TemplateSelector.vue'
import TemplateView from '@/components/TemplateView.vue'
import LabelEdit from '@/components/LabelEdit.vue'
import api from '@/common/api'


export default {
  name: 'Application',
  data () { return {
    application: {
      id: '',
      name: '',
      templates: ['Product', 'List', 'Blog Post']
    },
    url: '',
    template: ''
  }},

  components: {
    TemplateSelector,
    TemplateView,
    LabelEdit
  },
  methods: {
    updateUrl: function(newUrl) {
      this.url = newUrl
    },
    loadApplication: async function(id) {
      const application = await api.getApplication(id)
      if (!application) {
        await this.$router.replace('/?error=' + 'No application found under this id')
      }
      else {
        this.application.id = application.id
        this.application.name = application.name
      }
    },
    initApplication: async function() {
      const id = this.$route.params['id']
      if (!id)
        await this.createApplication()
      else
        await this.loadApplication(id)
    },
    updateName: async function(newName) {
      this.name = newName
      await api.renameApplication(this.application.id, newName)
    },
    createApplication: async function() {
      console.log('No id in params, creating the application')
      const result = await api.createApplication()
      await this.$router.replace(`/application/${result.id}`)
    }
  },
  created: async function() {
      await this.initApplication()
  },
  watch: {
    $route(to) {
      this.loadApplication(to.params['id'])
    }
  }
}
</script>
