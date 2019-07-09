<template>
  <div>
    <ul id="example-1">
      <li v-for="game in games" v-bind:key="game">
        {{ game.name }}
      </li>
    </ul>
  </div>
</template>

<script>
import mapRFunctions from '../../lib/MapRFunctions.js'

export default {
  name: 'games',
  data: function(){
    mapRFunctions.getGames().then(r => {
      self.games = r.data;
    })
    .catch(() => {})
    .finally(() =>{})
    return {
      games: []
    };
  },
  components: {
  }
}
</script>