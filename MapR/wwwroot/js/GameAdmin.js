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
                var self = this;
                var marker = {
                    name: self.markerName,
                    description: self.description,
                    customCSS: self.customCSS,
                    gameId: self.gameId,
                    mapId: self.mapId
                };
                connection.invoke('CreateMarker', marker)
                    .then(() => {
                        $('#newMarkerModal').modal('hide');
                        self.emptyForm();
                    });

            },
            checkIsEmpty: function(event) {
            },
            emptyForm: function(){
                this.markerName = '';
                this.customCSS = '';
                this.description = '';
            }
        }
    });
}

var mapRApp, global;
var gameAdmin = function (gameId, mapId) {
    
    var newMapUrl = '/games/'+gameId+'/maps/AddMap';
    var mountedFn = function(self){
        setUpMarkerDrag(document.querySelector("#mapVue"), self);
    }
    mapRApp = addToGame(gameId, mountedFn);

    setUpNewMapForm(newMapUrl);
    var markerForm = setUpNewMarkerForm(mapRApp.connection);
    markerForm.gameId = gameId;
    markerForm.mapId = mapId;

    function setUpMarkerDrag(container, mapRApp){
        var dragItem;

        var active = false;
        var currentX;
        var currentY;
        var inElementX;
        var inElementY;
        container.addEventListener("mousedown", dragStart, false);
        container.addEventListener("mouseup", dragEnd, false);
        container.addEventListener("mousemove", drag, false);

        var mapTransform = null;
        function dragStart(e) {
            mapTransform = mapRApp.mapZoom.getTransform();

            if (e.target.classList.contains('marker')) {
                if (e.type === "touchstart") {
                    var bb = e.target.getBoundingClientRect();
                    // initialX = e.touches[0].clientX - xOffset;
                    // initialY = e.touches[0].clientY - yOffset;
                    inElementX = e.center.x - bb.left;
                    inElementY = e.center.y - bb.top;
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

                mapRApp.markers[dragItem.id].x = (inElementX - mapTransform.x - mapRApp.getMap().offsetLeft)/mapTransform.scale;
                mapRApp.markers[dragItem.id].y = (inElementY - mapTransform.y - mapRApp.getMap().offsetTop)/mapTransform.scale;
                mapRApp.connection.invoke("MoveMarker", dragItem.id, mapRApp.markers[dragItem.id].x, mapRApp.markers[dragItem.id].y);

                active = false;
            }
        }

        function drag(e) {
            if (active) {
                e.preventDefault();
                if (e.type === "touchmove") {
                    currentX = e.touches[0].clientX - inElementX;
                    currentY = e.touches[0].clientY - inElementY;
                } else {
                    currentX = e.clientX - inElementX;
                    currentY = e.clientY - inElementY;
                }

                setTranslate(currentX, currentY, dragItem);
            }
        }

        function setTranslate(xPos, yPos, el) {
            var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ xPos + ', ' + yPos + ')';
            el.style.transform = transformValue; 
        }
    }
};