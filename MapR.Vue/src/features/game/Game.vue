<template>
  <div>
    <div class="mapContainer">
      <img v-bind:src="imageUrl" class="map"/>
    </div>
    <Marker 
        v-for="marker in store.state.markers"
        v-bind:key="marker.id"
        v-bind:marker="marker">
    </Marker>
  </div>
</template>

<script>
import * as signalR from '@aspnet/signalr';
import mapRFunctions from '../../lib/MapRFunctions.js'
import config from '../../../config.json';
import { store } from '../shared/store.js'
import * as panzoom from 'panzoom';

export default {
  name: 'game',
  props:{
    id: String
  },
  components: {
  },
  mounted: function() {
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
      store: store,
      game: null,
      imageUrl: '',
      mapZoom: null
    };
  },
  computed:{
    map: function(){
      return this.$el.querySelector('.map'); 
    }
  },
  mounted: function(){
    this.mapZoom = panzoom(this.getMap(),{
          maxZoom: 1,
          smoothScroll: false,
          minZoom: .1
      });
      this.mapZoom.on('transform', function(){
          //$('.marker').popover('hide');
          //resetMapMarkers(self.markers, self.mapZoom, self.getMap());
      });
  },
  methods:{
    connect: function(gameId){
      let self = this;
      
      let connection;// = store.getSignalRConnection(); 

      mapRFunctions.negotiateSignalr().then(resp => {
        let con = resp.data;
        const options = {
            accessTokenFactory: () => con.accessToken
        };
        connection = new signalR.HubConnectionBuilder()
          .withUrl(con.url, options)
          .configureLogging(signalR.LogLevel.Debug)
          .build();

        connection.on("SetAllMapMarkers", function(markers){
            for(var i = 0; i< markers.length; i++){
              self.addMarker(markers[i]);
            }
        });

        connection.start()
          .then(function () { 
            self.store.addToGame(gameId)
        });
      });
    },
    addMarker: function(marker){
        var self = this;
        self.store.addMarker(marker, self.mapZoom, this.map);
        //self.setMarker(marker, self.mapZoom, map);
    },
    setMarkerPosition: function(marker, mapZoom, mapElement) {
        var mapTransform = mapZoom.getTransform();
        var element = this.$el.querySelector('#' + marker.id);
        var markerX = marker.x,
            markerY = marker.y,
            left = mapTransform.scale * markerX + mapTransform.x + mapElement.offsetLeft,
            top = mapTransform.scale * markerY + mapTransform.y + mapElement.offsetTop;

        var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ left + ', ' + top + ')';
        element.style.transform = transformValue;
    }
  },
  watch: {
  }
}
</script>
