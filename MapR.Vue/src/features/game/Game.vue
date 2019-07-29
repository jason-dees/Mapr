<template>
  <div>
    <img v-bind:src="imageUrl" />
  </div>
</template>

<script>
import * as signalR from '@aspnet/signalr';
import mapRFunctions from '../../lib/MapRFunctions.js'
import config from '../../../config.json';
import { store } from '../shared/store.js'

export default {
  name: 'game',
  props:{
    id: String
  },
  components: {
  },
  data: function(){
    let self = this;
    mapRFunctions.getGame(self.id).then(r => {
      self.$set(self, 'game', r.data);
      self.$set(self, 'imageUrl', config.mapRFunctionsUrl + 'api/games/'+ self.game.id + '/activemap/image');
      store.setPageTitle(self.game.name);
      self.connect(self.game.id);
    });
    return {
      game: null,
      imageUrl: '',
      signalrData: null
    };
  },
  methods:{
    connect: function(gameId){
      let connection = store.getSignalRConnection(); 
      connection.start()
        .then(function () { connection.invoke("AddToGame", gameId) });
    }
  },
  watch: {
  }
}
</script>
