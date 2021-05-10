<template>
  <div class="full-height full-width">
    <iframe class="full-height full-width" v-bind:srcdoc="html"/>
  </div>
</template>

<script>
export default {
  name: 'TemplateView',
  props: {
    html: String
  }
}
</script>

<style scoped>
</style>
