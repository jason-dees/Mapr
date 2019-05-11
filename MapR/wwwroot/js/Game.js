function addToGame(gameId, mountedFn){

    mountedFn = mountedFn || function(){}; 

    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub", { accessTokenFactory: () => Window.loginToken })
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