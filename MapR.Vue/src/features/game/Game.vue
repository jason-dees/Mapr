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
    });
    mapRFunctions.negotiateSignalr().then(r => { self.signalrData = r.data; });
    return {
      game: null,
      imageUrl: '',
      signalrData: null
    };
  },
  watch: {
    signalrData: function(newData){
      console.log(signalR)
      let connection = new signalR.HubConnectionBuilder()
        .withUrl(newData.url)
        .build();
    }
  }
}
</script>
