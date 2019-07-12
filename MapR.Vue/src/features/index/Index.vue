<template>
  <div>
      <MapRNav v-bind:functionsUrl="config.mapRFunctionsUrl"
              v-bind:appServerUrl="appServer"
              v-bind:userInfo="userInfo"/>
    <div id="app">
      <router-view>
      this is the index
      </router-view>
      </div>
  </div>
</template>

<script>
import MapRNav from '../nav/MapRNav.vue'
import mapRFunctions from '../../lib/MapRFunctions.js'
const config = require('../../../config.json')

export default {
  name: 'index',
  data: function() {
    let self = this;
    mapRFunctions.getUser().then((r) => {
      self.userInfo.user = r.data.name;
      self.userInfo.loadedUserInfo = true;
    }).catch(() => {
      self.userInfo.user = null;
      self.userInfo.loadedUserInfo = false;
    }).finally(() => {
      self.userInfo.loadingUserInfo = false;
    });


    return  {
      userInfo: {
        loadedUserInfo: false,
        loadingUserInfo: true,
        user: '',
      },
      games: [],
      activeGame: null,
      config: config,
      appServer: window.location.href
    }
  },
  components: {
    MapRNav
  }
}
</script>

<style lang="scss">
  @import '../shared/main.scss';

  #app {
    font-family: 'Avenir', Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;
    padding-top:$menu-height;
  }
</style>
