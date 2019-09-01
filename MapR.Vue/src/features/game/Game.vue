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
      markers: []
    };
  },
  methods:{
    connect: function(gameId){
      let connection;// = store.getSignalRConnection(); 
      mapRFunctions.negotiateSignalr().then(resp => {
        let con = resp.data;
        const options = {
            accessTokenFactory: () => con.accessToken
        };
        connection = new signalR.HubConnectionBuilder()
          // .withUrl(config.mapRFunctionsUrl +'api')
          .withUrl(con.url, options)
          .configureLogging(signalR.LogLevel.Debug)
          .build();
        window.connection = connection;
        connection.on("SetAllMapMarkers", function(markers){
            console.log(markers, markers.length);
        });

        connection.start()
          .then(function () { 
            store.addToGame(gameId)
        });
      });

    },
    addMarker: function(marker){
        var self = this;
        this.$set(this.markers, marker.id, marker);
        this.$nextTick(function () {
            //setMarker(marker, self.mapZoom, self.getMap());
        })
    }
  },
  watch: {
  }
}
</script>
