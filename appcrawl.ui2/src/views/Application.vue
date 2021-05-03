<template>
  <div :style="{height: '100vh'}">

    <div id="title" :style="{height: '60px'}">
      <h4><label-edit :title="title"></label-edit></h4>
    </div>

    <div class="columns">
      <div class="full-height column is-one-fifth"><template-selector :templates="templates"/></div>
      <div class="full-height"><template-view/></div>
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
import LabelEdit from 'label-edit'
import api from '@/common/api'


export default {
  name: 'Application',
  data () { return {
    title: 'Cats',
    templates: ['Product', 'List', 'Blog Post']
  }},
  components: {
    TemplateSelector,
    TemplateView,
    LabelEdit
  },
  methods: {
    loadApplication: function(application) {
      this.title = application.title
    }
  },
  created: async function() {
    const params = this.$route.params
    if (!('id' in params)) {
      const result = await api.createApplication()
      this.$router.push(`application/${result.id}`)
    }
    else {
      const id = params['id']
      const application = api.getApplication(id)
      if (!application) {
        this.$router.push('/?error=' + 'No application found under this id')
      }
      else {
        this.loadApplication(application)
      }
    }

  }
}
</script>