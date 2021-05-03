<template>
  <nav class="full-height panel" id="templateSelector">
    <a class="panel-block" v-for="template in templates" :key="template">
      {{template}}
    </a>
  </nav>
</template>

<script>
export default {
  name: 'TemplateSelector',
  props: {
    templates: Array
  }
}
</script>

<style scoped>
  #templateSelector {
    background-color: rgba(211, 211, 211, 0.205);
    padding: 10px;
  }

</style>
