<template>
  <nav class="full-height panel" id="templateSelector">
    <div v-bind:class="{'is-active': selected && template.id === selected.id}"
       class = "panel-block"
       v-for="template in templates"
       :key="template.id">
      <span v-on:click="select(template)">
        {{template.name}}
      </span>
      <span v-on:click="remove">
        delete
      </span>
    </div>
    <button class="button" v-on:click="createTemplate">
      New Template
    </button>
  </nav>
</template>

<script>
export default {
  name: 'TemplateSelector',
  props: {
    templates: Array,
    selected: Object
  },
  data: function () {
    return {
    }
  },
  methods: {
    select: function(template) {
      this.$emit('select', template)
    },
    createTemplate: function () {
      this.$emit('create')
    },
    remove(template) {
      this.$emit('remove')
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
