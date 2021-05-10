<template>
  <div :style="{height: '100vh'}">

    <div id="title" :style="{height: '60px'}">
      <h4>
        <label-edit v-bind:text="application.name" v-on:update="renameApplication"></label-edit>
      </h4>
    </div>

    <div class="columns full-height">
      <div class="full-height column is-one-fifth">
        <template-selector
                v-bind:selected="template"
                v-bind:templates="application.templates"
                v-on:create="createTemplate"
                v-on:select="redirectToTemplate"
                v-on:remove="removeTemplate"
        />
      </div>
      <div v-if="template" class="full-height full-width">
        <h4><label-edit v-bind:text="template.name" v-on:update="renameTemplate"></label-edit></h4>
        <label-edit v-model:text="url" placeholder="Type URL here"></label-edit>
        <template-view  v-bind:html="html"/>
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
      templates: []
    },
    url: '',
    html: '',
    template: null
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

    removeTemplate(template) {
      api.removeTemplate()
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

  asyncComputed: {
    async html() {
      return this.url
             ? (await api.getHtml(this.url)).html
             : ''
    }
  },

  watch: {
    $route: async function(to) {
      await this.init(to)
    },
  }
}
</script>
