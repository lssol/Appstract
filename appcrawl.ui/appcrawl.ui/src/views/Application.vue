<template>
    <div class="row">
      <p>title</p>
    </div>
    <!-- <div class="row">
      <div class="column-20"><template-selector/></div>
      <div class="column-80"><template-view/></div>
    </div> -->
      <multipane>
        <div :style="{width: '20%'}">
          <template-selector />
        </div>  
        <multipane-resizer />
        <div :style="{width: '80%'}">
          <template-view />
        </div>  
      </multipane>
</template>


<style scoped>

</style>

<script>
import TemplateSelector from '@/components/TemplateSelector.vue'
import TemplateView from '@/components/TemplateView.vue'

export default {
  name: 'Application',
  components: {
    TemplateSelector,
    TemplateView
  }
}
</script>
    TemplateView