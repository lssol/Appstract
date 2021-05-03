<template>
  <div id="app">
    <div v-if="errorMessage" class='message is-danger'>
      {{ errorMessage }}
    </div>
    <router-view/>
  </div>
</template>

<script>
export default {
  data: {

  },
  computed: {
    errorMessage: function() {
      const params = this.$route.params
      if ('error' in params)
        return params['error']
      else 
        return ""
    }
  }
}
</script>

<style>
  .full-height {
    height: 100%;
  }
</style>
