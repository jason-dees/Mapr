<template>
  <div id="mapVue">
    <div class="mapContainer">
      <img v-bind:src="imageUrl" class="map"/>
    </div>
    <map-marker 
        v-for="marker in markers"
        v-bind:key="marker.Id"
        v-bind:marker="marker" />
  </div>
</template>

<script>
import * as signalR from '@aspnet/signalr';
import mapRFunctions from '../../lib/MapRFunctions.js'
import config from '../../../config.json';
import { store } from '../shared/store.js'
import * as panzoom from 'panzoom';
import MapMarker from './MapMarker.vue';

export default {
  name: 'Game',
  props:{
    id: String
  },
  components: {
    'map-marker': MapMarker
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
    },
    markers: function(){
      return this.store.state.game.markers;
    }
  },
  mounted: function(){
    var self = this;
    self.mapZoom = panzoom(self.map,{
        maxZoom: 1,
        smoothScroll: false,
        minZoom: .1
    });
    self.mapZoom.on('transform', function(){
        //$('.marker').popover('hide');
        for (var marker in self.markers) {
          self.setMarkerPosition(self.markers[marker], self.mapZoom, self.map);
        }
    });

    setUpMarkerDrag(document.querySelector("#mapVue"), self);
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
        
        connection.on("SetGameData", function(gameData){
          var markers = gameData.markers;
          self.store.resetGame();
          self.store.setIsGameOwner(gameData.isGameOwner);
          for(var i = 0; i< markers.length; i++){
            self.addMarker(markers[i]);
          }
        });
        connection.on("SetGameAdmin", function(data){
          console.log(data);
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
        this.$nextTick(function () {
          self.setMarkerPosition(marker, self.mapZoom, self.map);
        })
    },
    setMarkerPosition: function(marker, mapZoom, mapElement) {
        var mapTransform = mapZoom.getTransform();
        var element = this.$el.querySelector('#' + marker.Id);
        var markerX = marker.X,
            markerY = marker.Y,
            left = mapTransform.scale * markerX + mapTransform.x + mapElement.offsetLeft,
            top = mapTransform.scale * markerY + mapTransform.y + mapElement.offsetTop;

        var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ left + ', ' + top + ')';
        element.style.transform = transformValue;
    }
  },
  watch: {
  }
}
function setUpMarkerDrag(container, mapRApp){
        let dragItem;

        let active = false;
        let currentX;
        let currentY;
        let inElementX;
        let inElementY;
        container.addEventListener("touchstart", dragStart, false);
        container.addEventListener("touchend", dragEnd, false);
        container.addEventListener("touchmove", drag, false);

        container.addEventListener("mousedown", dragStart, false);
        container.addEventListener("mouseup", dragEnd, false);
        container.addEventListener("mousemove", drag, false);

        let mapTransform = null;
        function dragStart(e) {
            mapTransform = mapRApp.mapZoom.getTransform();

            if (e.target.classList.contains('marker') ) {
                if (e.type === "touchstart") {
                    inElementX = 0;
                    inElementY = 0;

                } else {
                    inElementX = e.layerX;
                    inElementY = e.layerY;
                }

                dragItem = e.target;
                active = true;
            }
        }

        function dragEnd(e) {
            if(active){
                inElementX = currentX;
                inElementY = currentY;

                mapRApp.markers[dragItem.id].x = (inElementX - mapTransform.x - mapRApp.map.offsetLeft)/mapTransform.scale;
                mapRApp.markers[dragItem.id].y = (inElementY - mapTransform.y - mapRApp.map.offsetTop)/mapTransform.scale;
                //THIS NEEDS TO BE FIXED PROBABLY
                mapRApp.connection.invoke("MoveMarker", 
                  dragItem.id, 
                  mapRApp.markers[dragItem.id].x, 
                  mapRApp.markers[dragItem.id].y);

                active = false;
                
            }
        }

        function drag(e) {
            if (active) {
                e.preventDefault();
                if (e.type === "touchmove") {
                    currentX = e.touches[0].clientX -inElementX;
                    currentY = e.touches[0].clientY - inElementY;
                } else {
                    currentX = e.clientX - inElementX;
                    currentY = e.clientY - inElementY;
                }

                setTranslate(currentX, currentY, dragItem);
            }
        }

        function setTranslate(xPos, yPos, el) {
            //$(el).popover('update');
            let transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ xPos + ', ' + yPos + ')';
            el.style.transform = transformValue;
        }
    }

</script>
