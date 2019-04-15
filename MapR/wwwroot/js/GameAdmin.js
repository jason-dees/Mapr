"use strict";

function setUpNewMapForm(newMapUrl) {
    var newMap = new Vue({
        el: "#newMapModal",
        data: {
            mapName: "",
            image: null,
            imagePreview: "",
            nameErrorMessage: "",
            formErrorMessage: ""
        },
        methods: {
            submit: function (event) {
                let self = this;

                if (this.checkIsEmpty()) return;
                if (self.image == null) return;

                let formData = new FormData();
                formData.append("ImageData", self.image);
                formData.set("Name", self.mapName);

                const config =  { headers: {'Content-Type': 'multipart/form-data' }};
                axios({
                    method: 'post',
                    url: newMapUrl,
                    data: formData,
                    config: config
                }).then(function (response) {
                    self.formErrorMessage = "";
                }).catch(function (error) {
                    self.formErrorMessage = error.message;
                });
            },
            checkIsEmpty: function () {
                if (this.mapName.trim().length == 0) {
                    this.nameErrorMessage = "Map needs a name!";
                    return true;
                }
                else {
                    this.nameErrorMessage = "";
                    return false;
                }
            },
            fileSelected: function(event){
                let self = this;
                self.imagePreview = "";
                self.image = null;

                if(event.target.files.length == 0) return;
                self.image = event.target.files[0];

                let fileReader = new FileReader();
                fileReader.addEventListener('load', function(){
                    self.imagePreview = fileReader.result;
                }, false);
                fileReader.readAsDataURL(self.image);
            }
        }
    });
}

function setUpAdminMap() {

}

function setUpNewMarkerForm(connection){
    return new Vue({
        el: '#newMarkerModal',
        data:{
            markerName: "",
            customCSS: "",
            description: "",
            nameErrorMessage: "",
            formErrorMessage: "",
            gameId: null,
            mapId: null
        },
        methods:{
            submit: function() {
                var marker = {
                    id: "STUFFISHERE",
                    gameId: this.gameId,
                    mapId: this.mapId
                };
                connection.invoke('CreateMarker', marker);
            },
            checkIsEmpty: function(event) {

            }
        }
    });
}


var mapRApp, mapZoom;
var gameAdmin = function (gameId, mapId) {
    
    //something is going on with this not working?
    var newMapUrl = '/games/'+gameId+'/maps/AddMap';

    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub", { accessTokenFactory: () => this.loginToken })
        .build();

    connection.start()
        .then(function () { connection.invoke("AddToGame", gameId) })
        .then(function () { connection.invoke("SendAllMarkers", gameId, mapId)});

    function resetMapMarkers() {
        for (var marker in mapRApp.markers) {
            setMarker(mapRApp.markers[marker]);
        }
    }

    mapRApp = new Vue({
        el: '#gameAdminVue',
        data:{
            markers: {},
            maps: [],
            activeMap: '/game/' + gameId + '/maps/' + mapId
        },
        methods:{
            addMarker: function(marker){
                this.$set(this.markers, marker.id, marker);
                this.$nextTick(function () {
                    setMarker(marker);
                })
            },
            getMarker: function (id){
                return this.markers[id];
            }
        },
        mounted: function(){
            mapZoom = panzoom(document.querySelector('#map'),{
                maxZoom: 1,
                smoothScroll: false,
                minZoom: .1
            });

            mapZoom.on('transform', resetMapMarkers);
        }
    });

    function setMarker(marker) {
        var mapTransform = mapZoom.getTransform();
        var element = document.querySelector('#' + marker.id);
        var mapElement = document.querySelector('#map');
        var markerX = marker.x,
            markerY = marker.y,
            left = mapTransform.scale * markerX + mapTransform.x + mapElement.offsetLeft,
            top = mapTransform.scale * markerY + mapTransform.y + mapElement.offsetTop;

        var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ left + ', ' + top + ')';
        element.style.transform = transformValue;
    }

    connection.on("SetMarker", mapRApp.addMarker);
    setUpNewMapForm(newMapUrl);
    var markerForm = setUpNewMarkerForm(connection);
    markerForm.gameId = gameId;
    markerForm.mapId = mapId;
};