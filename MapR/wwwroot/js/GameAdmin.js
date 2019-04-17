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

var mapRApp, mapZoom, global;
var gameAdmin = function (gameId, mapId) {
    
    var newMapUrl = '/games/'+gameId+'/maps/AddMap';

    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/mapHub", { accessTokenFactory: () => this.loginToken })
        .build();

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
                    setMarker(marker, mapZoom);
                })
            },
            getMarker: function (id){
                return this.markers[id];
            }
        },
        mounted: function(){
            var self = this;
            mapZoom = panzoom(document.querySelector('#map'),{
                maxZoom: 1,
                smoothScroll: false,
                minZoom: .1
            });

            mapZoom.on('transform', function(){resetMapMarkers(self.markers, mapZoom);});
            setUpMarkerDrag();

            connection.start()
                .then(function () { connection.invoke("AddToGame", gameId) })
                .then(function () { connection.invoke("SendAllMarkers", gameId, mapId)});
        }
    });

    function setUpMarkerDrag(){
        var dragItem;
        var container = document.querySelector("#mapVue");
    
        var active = false;
        var currentX;
        var currentY;
        var initialX;
        var initialY;
        var xOffset = document.querySelector('#map').offsetLeft;
        var yOffset = document.querySelector('#map').offsetTop;
        container.addEventListener("mousedown", dragStart, false);
        container.addEventListener("mouseup", dragEnd, false);
        container.addEventListener("mousemove", drag, false);
    
        var mapTransform = null;
        function dragStart(e) {
            mapTransform = mapZoom.getTransform();

            if (e.target.classList.contains('marker')) {
                if (e.type === "touchstart") {
                    var bb = e.target.getBoundingClientRect();
                    // initialX = e.touches[0].clientX - xOffset;
                    // initialY = e.touches[0].clientY - yOffset;
                    initialX = e.center.x - bb.left;
                    initialY = e.center.y - bb.top;
                } else {
                    initialX = e.layerX;
                    initialY = e.layerY;
                }

                dragItem = e.target;
                active = true;
            }
        }
    
        function dragEnd(e) {
            if(active){
                initialX = currentX;
                initialY = currentY;

                mapRApp.markers[dragItem.id].x = (initialX - mapTransform.x - document.querySelector('#map').offsetLeft)/mapTransform.scale;
                mapRApp.markers[dragItem.id].y = (initialY - mapTransform.y - document.querySelector('#map').offsetTop)/mapTransform.scale;
                connection.invoke("MoveMarker", dragItem.id, mapRApp.markers[dragItem.id].x, mapRApp.markers[dragItem.id].y);

                active = false;
            }
        }
    
        function drag(e) {
            if (active) {
                global = e;
                e.preventDefault();
                if (e.type === "touchmove") {
                    currentX = e.touches[0].clientX - initialX;
                    currentY = e.touches[0].clientY - initialY;
                } else {
                    currentX = e.clientX - initialX;
                    currentY = e.clientY - initialY;
                }

                xOffset = currentX;
                yOffset = currentY;

                setTranslate(currentX, currentY, dragItem);
            }
        }
    
        function setTranslate(xPos, yPos, el) {
            var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ xPos + ', ' + yPos + ')';
            el.style.transform = transformValue; 
        }
    }

    connection.on("SetMarker", mapRApp.addMarker);
    connection.on("SetAllMapMarkers", function(markers){
        for(var i = 0; i< markers.length; i++){
            mapRApp.addMarker(markers[i]);
        }
    });
    setUpNewMapForm(newMapUrl);
    var markerForm = setUpNewMarkerForm(connection);
    markerForm.gameId = gameId;
    markerForm.mapId = mapId;
};