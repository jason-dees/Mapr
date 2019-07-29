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

      connection.on("SetAllMapMarkers", function(markers){
          console.log(markers);
          for(var i = 0; i< markers.length; i++){
              vue.addMarker(markers[i]);
          }
      });

      connection.start()
        .then(function () { 
          console.log("started")
          connection.invoke("AddToGame", gameId) 
          connection.invoke("SendAllMapMarkers");
      });



    }
  },
  watch: {
  }
}
</script>
