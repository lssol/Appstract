<template>
  <nav class="full-height panel" id="templateSelector">
    <a v-bind:class="{'is-active': template === vSelectedTemplate}"
       class = "panel-block"
       v-on:click="select(template)"
       v-for="template in templates"
       :key="template">
      {{template}}
    </a>
  </nav>
</template>

<script>
export default {
  name: 'TemplateSelector',
  props: {
    templates: Array,
    selectedTemplate: String
  },
  data: function () {
    return {
      vSelectedTemplate: this.selectedTemplate
    }
  },
  methods: {
    select: function(template) {
      this.vSelectedTemplate = template
      this.$emit('input', template)
    }
  }

}
</script>

<style scoped>
  #templateSelector {
    background-color: rgba(211, 211, 211, 0.205);
    padding: 10px;
  }
  .is-active {
    font-weight: bold;
  }
</style>
