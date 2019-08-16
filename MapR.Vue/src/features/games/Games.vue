<template>
  <div>
    <ul id="example-1">
      <li v-for="game in gamesList" v-bind:key="game.id">
        <router-link v-bind:to="'/games/'+game.id">{{ game.name }}</router-link>
      </li>
    </ul>
  </div>
</template>

<script>
import mapRFunctions from '../../lib/MapRFunctions.js'

export default {
  // props: {
  //   gamesList: Array
  // },
  data: function(){
    let self = this;
    mapRFunctions.getGames().then(r => {
      self.$set(self, 'gamesList', r.data);
    })
    .catch(() => {})
    .finally(() =>{})
    return {
      gamesList: []
    };
  },
  components: {
  }
}
</script>