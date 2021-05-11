<template>
  <nav class="full-height" id="templateSelector">
    <div id='templates'>
      <div class="template columns" v-bind:class="{'is-active': selected && template.id === selected.id}"
           v-for="template in templates"
           :key="template.id">
        <div class="column is-four-fifth">
          <a class="label" v-on:click="select(template)">{{template.name}}</a>
        </div>
        <div class="column has-text-right is-one-fifth">
          <span class="delete" v-on:click="remove(template.id)">delete</span>
        </div>
      </div>
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
    remove(templateId) {
      this.$emit('remove', templateId)
    }
  }

}
</script>

<style scoped>
  #templateSelector {
    background-color: rgba(211, 211, 211, 0.205);
    padding: 10px;
    padding-top: 25px;
  }
  .is-active {
    font-weight: bold;
  }
  .label {
    font-weight: normal
  }
  #templates {
    margin-bottom: 50px;
  }
  .is-active {
    background-color: white;
  }
  .template {
    padding-left: 10px;
  }
  .template:hover {
    background-color: rgba(211, 211, 211, 0.21);
  }
  .column {
    padding: 5px;
  }
  .columns {
    margin: 0;
  }
</style>
