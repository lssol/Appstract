<template>
  <div class="full-height" id="templateSelector">
    <span class="pseudo button stack" v-for="template in templates" :key="template">
      {{template}}
    </span>
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

<style scoped>
  #templateSelector {
    background-color: rgba(211, 211, 211, 0.205);
    padding: 10px;
  }

</style>
