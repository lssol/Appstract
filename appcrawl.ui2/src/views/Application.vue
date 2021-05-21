<template>
  <div :style="{height: '100vh'}">

    <div id="title" :style="{height: '60px'}">
        <label-edit v-bind:text="application.name" v-on:update="renameApplication"></label-edit>
    </div>

    <div class="columns full-height">
      <div id="template-selector-container" class="full-height column is-one-fifth">
        <template-selector
                v-bind:selected="template"
                v-bind:templates="application.templates"
                v-on:create="createTemplate"
                v-on:select="redirectToTemplate"
                v-on:remove="removeTemplate"
        />
      </div>
      <div v-if="template" class="full-height full-width">
        <label-edit v-bind:text="template.name" v-on:update="renameTemplate"></label-edit>
        <label-edit v-bind:text="template.url" v-on:update="setUrl" placeholder="Type URL here"></label-edit>
        <em v-if="content_loading">Loading...</em>
        <div v-if="error" class="notification is-danger is-light">
          <button class="delete" v-on:click="removeError"></button>
          {{ error }}
        </div>
        <iframe v-if="template.html" class="full-height full-width" v-bind:srcdoc="template.html"/>
      </div>
    </div>
    
    <div id="model_creation">
      <button class="button is-primary">Create Model</button>
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
  #template-selector-container {
    padding-top: 0;
  }
  #model_creation {
    position: absolute;
    top: 10px;
    right: 10px;
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
      templates: []
    },
    template: null,
    content_loading: false,
    error: ""
  }},

  components: {
    TemplateSelector,
    TemplateView,
    LabelEdit
  },
  methods: {
    loadApplication: async function(id) {
      const application = await api.getApplication(id)
      if (!application) {
        await this.$router.replace('/?error=' + 'No application found under this id')
      }
      else {
        this.application.id = application.id
        this.application.name = application.name
        this.application.templates = application.templates
      }
    },
    
    removeError() {
      this.error = ''
    },
    
    async setUrl(newUrl) {
      this.error = ''
      this.template.url = newUrl
      this.content_loading = true
      console.log("Attempting to retrieve Html")
      try {
        const {html} = await api.setUrlTemplate(this.template.id, newUrl)
        this.template.html = html
      }
      catch(e) {
        this.error = "Could not load url"
        console.log("Problem when retrieving HTML from url", e)
      }
      finally {
        this.content_loading = false
      }
    },

    removeTemplate(id) {
      api.removeTemplate(id)
      this.application.templates = this.application.templates.filter(t => t.id !== id)
      this.template = null
    },

    renameApplication: function(newName) {
      if (newName) {
        this.application.name = newName
        api.renameApplication(this.application.id, newName)
      }
    },

    renameTemplate(newName) {
      if (newName) {
        this.template.name = newName
        api.renameTemplate(this.template.id, newName)
      }
    },

    redirectToTemplate(template) {
      this.$router.push(`/application/${this.application.id}?templateId=${template.id}`);
    },

    createTemplate: async function() {
      console.log("Creating a new Template in application " + this.application.name)
      const template = await api.createTemplate(this.application.id)
      this.redirectToTemplate(template)
    },

    async init(url) {
      const applicationId = url.params['applicationId']
      const templateId    = url.query['templateId']

      if (!applicationId) {
        console.log('No id in params, creating a new application')
        const newApplication = await api.createApplication()
        await this.$router.replace(`/application/${newApplication.id}`)
        return
      }

      // if (applicationId !== this.application.id)
        await this.loadApplication(applicationId)

      if (templateId)
        this.template = this.application.templates.find(t => t.id === templateId)
    }
  },

  created: async function() {
      await this.init(this.$route)
  },

  watch: {
    $route: async function(to) {
      await this.init(to)
    },
    template: function() {

    }
  }
}
</script>
