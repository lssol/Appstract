<template>
  <div class="full-height full-width">
    <iframe class="full-height full-width" v-bind:src="url"/>
  </div>
</template>

<script>
export default {
  name: 'TemplateView',
  props: {
    url: String
  }
}
</script>

<style scoped>
</style>
