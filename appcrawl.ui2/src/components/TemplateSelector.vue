<template>
  <div class="full-height" id="templateSelector">
    <p>selector</p>
  </div>
</template>

<script>
export default {
  name: 'TemplateSelector',
  props: {
    templates: Array
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
  #templateSelector {
    background-color: lightgray
  }
</style>
