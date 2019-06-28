<template>
  <div>
      <MapRNav v-bind:functionsUrl="config.mapRFunctionsUrl"
              v-bind:appServerUrl="appServer"/>
    <div id="app">
      <router-view>
      this is the index
      </router-view>
      </div>
  </div>
</template>

<script>
import MapRNav from '../nav/MapRNav.vue'
const config = require('../../../config.json')

export default {
  name: 'index',
  data: function(){
    return  {
      config: config,
      appServer: window.location.href
      }
  },
  components: {
    MapRNav
  }
}
</script>

<style>
body {
  margin:0px;
}
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
