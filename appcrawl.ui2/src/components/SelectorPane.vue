<!--

items contains an array where each item must have the following attributes:
-> id
-> name

The component emits:
- select(item.id)
- create()
- remove(item.id)

-->
<template>
  <nav> 
    <div id="title">{{ title }}</div>
    <div id='items'>
      <div
          v-for="item in items" :key="item.id"
          class="item columns" 
          v-bind:class="{'is-active': selected && item.id === selected.id}">
        
        <div class="column is-four-fifth">
          <a class="label" v-on:click="select(item)">{{item.name}}</a>
        </div>
        <div class="column has-text-right is-one-fifth">
          <span class="delete" v-on:click="remove(item)">delete</span>
        </div>
      </div>
    </div>
    <button class="button" v-on:click="create">
      New 
    </button>
  </nav>
</template>

<script>
export default {
  name: 'SelectorPane',
  props: {
    items: Array,
    selected: Object,
    title: ""
  },
  data: function () {
    return { }
  },
  methods: {
    select: function(item) {
      this.$emit('select', item)
    },
    create: function () {
      this.$emit('create')
    },
    remove(item) {
      this.$emit('remove', item)
    }
  }

}
</script>

<style scoped>
  nav {
    background-color: rgba(211, 211, 211, 0.205);
    padding: 10px;
  }
  #title {
    color: #3e3e3e;
    font-size: 80%;
    font-weight: bold;
    padding-bottom: 10px;
  }
  .is-active {
    font-weight: bold;
  }
  .label {
    font-weight: normal
  }
  #items {
    margin-bottom: 15px;
  }
  .is-active {
    background-color: white;
  }
  .item {
    padding-left: 10px;
  }
  .item:hover {
    background-color: rgba(211, 211, 211, 0.21);
  }
  .column {
    padding: 5px;
  }
  .columns {
    margin: 0;
  }
</style>
