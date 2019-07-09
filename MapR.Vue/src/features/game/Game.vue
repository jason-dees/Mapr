<template>
  <div>
    <img v-bind:src="imageUrl" />
  </div>
</template>

<script>
import mapRFunctions from '../../lib/MapRFunctions.js'
import config from '../../../config.json';

export default {
  name: 'game',
  props:{
    id: String
  },
  components: {
  },
  data: function(){
    console.log(config);
    let self = this;
    mapRFunctions.getGame(self.id).then(r => {
      self.$set(self, 'game', r.data);
      self.$set(self, 'imageUrl', config.mapRFunctionsUrl + 'api/games/'+ self.game.id + '/activemap/image');
      console.log(r.data);
    });
    return {
      game: null,
      imageUrl: ''
    };
  }
}
</script>
