<template>
  <div>
    <h1 class="title is-1">Applications</h1>
    <div v-for="app in applications" :key="app.id">
      <router-link :to="{name: 'Application', params: {applicationId: app.id}}">{{ app.name }}</router-link>
      -
      <a class="delete" v-on:click="remove(app.id)">delete</a>
    </div>
    <router-link :to="{name: 'Application'}" class="button">New</router-link>
  </div>
  
</template>

<script>
import api from '@/common/api'

export default {
    data: function() {
      return {
        applications: []
      }
    },
    async created() {
      await this.load()
    },
    methods: {
      async load() {
        this.applications = await api.getApplications()
      },
      async remove(id) {
        await api.removeApplication(id)
        await this.load()
      }
    }
}
</script>

<style>

</style>