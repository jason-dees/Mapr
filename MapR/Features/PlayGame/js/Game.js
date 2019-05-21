export function addToGame(gameId, mountedFn){
    mountedFn = mountedFn || function(){}; 

    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub")
        .build();
    var vue = new Vue({
        el: '#gameVue',
        data:{
            markers: {},
            maps: [],
            activeMapId: '',
            mapZoom: null,
            connection: connection,
            gameId : gameId 
        },
        computed: {
            activeMapUrl: function(){
                return '/game/'+gameId+'/maps/' + this.activeMapId;
            }
        },
        methods:{
            addMarker: function(marker){
                var self = this;
                this.$set(this.markers, marker.id, marker);
                this.$nextTick(function () {
                    setMarker(marker, self.mapZoom, self.getMap());
                })
            },
            getMarker: function (id){
                return this.markers[id];
            },
            setMap: function(mapId){
                this.connection.invoke('ChangeMap', gameId, mapId);
            },
            getMap: function(){
                return this.getEle('.map');
            },
            getEle: function(query){
                return this.$el.querySelector(query);
            },
            moveToMarker: function(marker){
                console.log(marker);
            }
        },
        mounted: function(){
            var self = this;
            self.mapZoom = panzoom(self.getMap(),{
                maxZoom: 1,
                smoothScroll: false,
                minZoom: .1
            });

            self.mapZoom.on('transform', function(){
                $('.marker').popover('hide');
                resetMapMarkers(self.markers, self.mapZoom, self.getMap());
            });

            connection.start()
                .then(function () { connection.invoke("AddToGame", gameId) })

            mountedFn(self);
        },
    });

    connection.on("SetMarker", vue.addMarker);
    connection.on("SetAllMapMarkers", function(markers){
        for(var i = 0; i< markers.length; i++){
            vue.addMarker(markers[i]);
        }
    });
    connection.on("SetMap", function(mapId){
        $('.marker').popover('hide');
        vue.activeMapId = mapId;
        vue.markers = {};
    });
    return vue;
}
export function setMarker(marker, mapZoom, mapElement) {
    var mapTransform = mapZoom.getTransform();
    var element = document.querySelector('#' + marker.id);
    var markerX = marker.x,
        markerY = marker.y,
        left = mapTransform.scale * markerX + mapTransform.x + mapElement.offsetLeft,
        top = mapTransform.scale * markerY + mapTransform.y + mapElement.offsetTop;

    var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ left + ', ' + top + ')';
    element.style.transform = transformValue;
}

export function resetMapMarkers(markers, map, mapElement) {
    for (var marker in markers) {
        setMarker(markers[marker], map, mapElement);
    }
}
//Do a MapVue Component here with display of markers and map

Vue.component('map-vue', {
    props:['mapSrc', 'markers'],
    data: function(){
        return {

        };
    },
    template:`
        <div>
            <div class="mapContainer">
                <img class="map" v-bind:src="mapSrc" style="min-width:100px;min-height:100px"/>
            </div>
            <map-marker-vue 
                v-for="marker in markers"
                v-bind:key="marker.id"
                v-bind:marker="marker">
            </map-marker-vue>
        </div>  
    `,
    mounted: function(){
    }
});

Vue.component('map-marker-vue', {
    props:['marker'],
    data: function(){
        return {
        };
    },
    computed: {
        computedCss: function(){
            var backgroundImage = 'background-image: url("' + this.marker.iconUrl + '?width=25");';
            return backgroundImage + this.marker.customCSS;
        }
    },
    mounted:function(){
        $('#' + this.marker.id).popover({
            container:'body',
            content: this.marker.description || "",
            title: this.marker.name
        });
    },
    template:`
        <div v-bind:style="computedCss" v-bind:id="marker.id" class="marker">
        </div>
    `
});
