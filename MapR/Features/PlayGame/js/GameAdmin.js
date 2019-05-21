"use strict";
import {addToGame} from "./Game";
export default function gameAdmin(gameId, mapId) {
    
    var mountedFn = function(self){
        setUpMarkerDrag(document.querySelector("#mapVue"), self);
    }
    let mapRApp = addToGame(gameId, mountedFn);

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

            if (e.target.classList.contains('marker') ) {
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

Vue.component('image-upload', {
    data: function(){
        return {
            imagePreview: "",
            image:null
        };
    },
    methods:{
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
            this.$emit('update:image', self.image)
        }
    },
    template: `
        <div>
            <input type="file" class="form-input"
                name="imageUpload" id="imageUpload"
                accept="image/png, image/jpeg"
                v-on:change="fileSelected" />
            <img id="imagePreview" v-bind:src="imagePreview" style="width:100%;" />
        </div>
    `
});

Vue.component('add-marker-vue', {
    props:['gameId', 'mapId', 'connection'],
    data: function(){
        return {
            markerName: "",
            customCSS: "",
            description: "",
            nameErrorMessage: "",
            formErrorMessage: "",
            imageData: null
        };
    },
    methods:{
        submit: function() {
            var self = this;

            var newMarkerUrl = '/games/' + self.gameId + '/maps/' + self.mapId + '/markers/AddMarker';

            let formData = new FormData();
            console.log(self.imageData)
            formData.append("ImageData", self.imageData);
            formData.set("Name", self.markerName);
            formData.set("Description", self.description);
            formData.set("CustomCSS", self.customCSS);

            const config =  { headers: {'Content-Type': 'multipart/form-data' }};
            axios({
                method: 'post',
                url: newMarkerUrl,
                data: formData,
                config: config
            }).then(function (response) {
                self.formErrorMessage = "";
            }).catch(function (error) {
                self.formErrorMessage = error.message;
            });

        },
        checkIsEmpty: function(event) {
        },
        emptyForm: function(){
            this.markerName = '';
            this.customCSS = '';
            this.description = '';
        }
    },
    template:`
    <div class="modal" role="form" id="newMarkerModal">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5>Add New Marker</h5>
                </div>
                <div class="modal-body" v-bind:class="{ 'alert-danger' : (formErrorMessage.length > 0) }">
                    <div class="form-group">
                        <input type="text" name="name" class="form-control"
                            placeholder="New Marker Name"
                            v-model="markerName"
                            v-bind:class="{ 'is-invalid' : (nameErrorMessage.length > 0) }"
                            v-on:keyup="checkIsEmpty" />
                    </div>

                    <div class="alert alert-warning" v-show="nameErrorMessage.length > 0">{{nameErrorMessage}}</div>

                    <div class="form-group">
                        <input type="text"
                            class="form-control"
                            placeholder="Custom CSS"
                            v-model="customCSS" />
                    </div>

                    <div class="form-group">
                        <label for="">Description</label>
                        <textarea class="form-control" rows="3" v-model="description"></textarea>
                    </div>

                    <image-upload v-bind:image.sync="imageData"></image-upload>

                    <div class="alert alert-warning" v-show="formErrorMessage.length > 0">{{formErrorMessage}}</div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-secondary" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-outline-primary" v-on:click="submit">Create</button>
                </div>
            </div>
        </div>
    </div>
    `,
    mounted: function(){
        global = this;
    }
});

Vue.component('add-map-vue', {
    props:['gameId'],
    data: function(){
        return {
            mapName: "",
            image: null,
            nameErrorMessage: "",
            formErrorMessage: ""
        };
    },
    methods: {
        submit: function (event) {
            let self = this;

            if (this.checkIsEmpty()) return;
            if (self.image == null) return;
            var newMapUrl = '/games/' + self.gameId + '/maps/AddMap';

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
        }
    },
    template:`
    <div class="modal" role="form" id="newMapModal">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5>Add New Map</h5>
                </div>
                <div class="modal-body" v-bind:class="{ 'alert-danger' : (formErrorMessage.length > 0) }">
                    <div class="form-group">
                        <input type="text" name="name" class="form-control"
                            placeholder="New Map Name"
                            v-model="mapName"
                            v-bind:class="{ 'is-invalid' : (nameErrorMessage.length > 0) }"
                            v-on:keyup="checkIsEmpty" />
                    </div>

                    <div class="alert alert-warning" v-show="nameErrorMessage.length > 0">{{nameErrorMessage}}</div>

                    <div class="form-group">
                        <label class="form-label" for="imageUpload">Map Image</label>
                        <image-upload v-bind:image.sync="image"></image-upload>
                    </div>
                    <div class="alert alert-warning" v-show="formErrorMessage.length > 0">{{formErrorMessage}}</div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-secondary" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-outline-primary" v-on:click="submit">Create</button>
                </div>
            </div>
        </div>
    </div>
    `
});