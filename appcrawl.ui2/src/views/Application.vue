<template>
  <div :style="{height: '100vh'}">

    <div id="title" :style="{height: '60px'}">
      <h4><label-edit :text="text"></label-edit></h4>
    </div>

    <div class="flex two full-height">
      <div class="fifth full-height"><template-selector/></div>
      <div class="full-height four-fifth"><template-view/></div>
    </div>

  </div>
</template>


<style scoped>
  #title {
    padding-left: 10px;
    padding-top: 10px;
  } 
  #title h4 {
    width: fit-content
  }
</style>

<script>
import TemplateSelector from '@/components/TemplateSelector.vue'
import TemplateView from '@/components/TemplateView.vue'
import LabelEdit from 'label-edit'


export default {
  name: 'Application',
  data () { return {
    text: 'Cats'
  }},
  components: {
    TemplateSelector,
    TemplateView,
    LabelEdit
  }
}
</script>