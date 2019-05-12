var gameAdmin =
/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./Features/PlayGame/js/GameAdmin.js");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./Features/PlayGame/js/Game.js":
/*!**************************************!*\
  !*** ./Features/PlayGame/js/Game.js ***!
  \**************************************/
/*! exports provided: addToGame, setMarker, resetMapMarkers */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"addToGame\", function() { return addToGame; });\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"setMarker\", function() { return setMarker; });\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"resetMapMarkers\", function() { return resetMapMarkers; });\nfunction addToGame(gameId, mountedFn){\n\n    mountedFn = mountedFn || function(){}; \n\n    let connection = new signalR.HubConnectionBuilder()\n        .withUrl(\"/mapHub\")\n        .build();\n    var vue = new Vue({\n        el: '#gameVue',\n        data:{\n            markers: {},\n            maps: [],\n            activeMapId: '',\n            mapZoom: null,\n            connection: connection,\n            gameId : gameId \n        },\n        computed: {\n            activeMapUrl: function(){\n                return '/game/'+gameId+'/maps/' + this.activeMapId;\n            }\n        },\n        methods:{\n            addMarker: function(marker){\n                var self = this;\n                this.$set(this.markers, marker.id, marker);\n                this.$nextTick(function () {\n                    setMarker(marker, self.mapZoom, self.getMap());\n                })\n            },\n            getMarker: function (id){\n                return this.markers[id];\n            },\n            setMap: function(mapId){\n                this.connection.invoke('ChangeMap', gameId, mapId);\n            },\n            getMap: function(){\n                return this.getEle('.map');\n            },\n            getEle: function(query){\n                return this.$el.querySelector(query);\n            }\n        },\n        mounted: function(){\n            var self = this;\n            self.mapZoom = panzoom(self.getMap(),{\n                maxZoom: 1,\n                smoothScroll: false,\n                minZoom: .1\n            });\n\n            self.mapZoom.on('transform', function(){\n                $('.marker').popover('hide');\n                resetMapMarkers(self.markers, self.mapZoom, self.getMap());\n            });\n\n            connection.start()\n                .then(function () { connection.invoke(\"AddToGame\", gameId) })\n\n            mountedFn(self);\n        },\n    });\n\n    connection.on(\"SetMarker\", vue.addMarker);\n    connection.on(\"SetAllMapMarkers\", function(markers){\n        for(var i = 0; i< markers.length; i++){\n            vue.addMarker(markers[i]);\n        }\n    });\n    connection.on(\"SetMap\", function(mapId){\n        $('.marker').popover('hide');\n        vue.activeMapId = mapId;\n        vue.markers = {};\n    });\n    return vue;\n}\nfunction setMarker(marker, mapZoom, mapElement) {\n    var mapTransform = mapZoom.getTransform();\n    var element = document.querySelector('#' + marker.id);\n    var markerX = marker.x,\n        markerY = marker.y,\n        left = mapTransform.scale * markerX + mapTransform.x + mapElement.offsetLeft,\n        top = mapTransform.scale * markerY + mapTransform.y + mapElement.offsetTop;\n\n    var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ left + ', ' + top + ')';\n    element.style.transform = transformValue;\n}\n\nfunction resetMapMarkers(markers, map, mapElement) {\n    for (var marker in markers) {\n        setMarker(markers[marker], map, mapElement);\n    }\n}\n//Do a MapVue Component here with display of markers and map\n\nVue.component('map-vue', {\n    props:['mapSrc', 'markers'],\n    data: function(){\n        return {\n\n        };\n    },\n    template:`\n        <div>\n            <div class=\"mapContainer\">\n                <img class=\"map\" v-bind:src=\"mapSrc\" style=\"min-width:100px;min-height:100px\"/>\n            </div>\n            <map-marker-vue \n                v-for=\"marker in markers\"\n                v-bind:key=\"marker.id\"\n                v-bind:marker=\"marker\">\n            </map-marker-vue>\n        </div>  \n    `,\n    mounted: function(){\n    }\n});\n\nVue.component('map-marker-vue', {\n    props:['marker'],\n    data: function(){\n        return {\n        };\n    },\n    computed: {\n        computedCss: function(){\n            var backgroundImage = 'background-image: url(\"' + this.marker.iconUrl + '?width=25\");';\n            return backgroundImage + this.marker.customCSS;\n        }\n    },\n    mounted:function(){\n        $('#' + this.marker.id).popover({\n            container:'body',\n            content: this.marker.description || \"\",\n            title: this.marker.name\n        });\n    },\n    template:`\n        <div v-bind:style=\"computedCss\" v-bind:id=\"marker.id\" class=\"marker\">\n        </div>\n    `\n});\n\n//# sourceURL=webpack://%5Bname%5D/./Features/PlayGame/js/Game.js?");

/***/ }),

/***/ "./Features/PlayGame/js/GameAdmin.js":
/*!*******************************************!*\
  !*** ./Features/PlayGame/js/GameAdmin.js ***!
  \*******************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* WEBPACK VAR INJECTION */(function(global) {/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"default\", function() { return gameAdmin; });\n/* harmony import */ var _Game__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./Game */ \"./Features/PlayGame/js/Game.js\");\n﻿\n\nfunction gameAdmin(gameId, mapId) {\n    \n    var mountedFn = function(self){\n        setUpMarkerDrag(document.querySelector(\"#mapVue\"), self);\n    }\n    let mapRApp = Object(_Game__WEBPACK_IMPORTED_MODULE_0__[\"addToGame\"])(gameId, mountedFn);\n\n    function setUpMarkerDrag(container, mapRApp){\n        var dragItem;\n\n        var active = false;\n        var currentX;\n        var currentY;\n        var inElementX;\n        var inElementY;\n        container.addEventListener(\"mousedown\", dragStart, false);\n        container.addEventListener(\"mouseup\", dragEnd, false);\n        container.addEventListener(\"mousemove\", drag, false);\n\n        var mapTransform = null;\n        function dragStart(e) {\n            mapTransform = mapRApp.mapZoom.getTransform();\n\n            if (e.target.classList.contains('marker') ) {\n                if (e.type === \"touchstart\") {\n                    var bb = e.target.getBoundingClientRect();\n                    // initialX = e.touches[0].clientX - xOffset;\n                    // initialY = e.touches[0].clientY - yOffset;\n                    inElementX = e.center.x - bb.left;\n                    inElementY = e.center.y - bb.top;\n                } else {\n                    inElementX = e.layerX;\n                    inElementY = e.layerY;\n                }\n\n                dragItem = e.target;\n                active = true;\n            }\n        }\n\n        function dragEnd(e) {\n            if(active){\n                inElementX = currentX;\n                inElementY = currentY;\n\n                mapRApp.markers[dragItem.id].x = (inElementX - mapTransform.x - mapRApp.getMap().offsetLeft)/mapTransform.scale;\n                mapRApp.markers[dragItem.id].y = (inElementY - mapTransform.y - mapRApp.getMap().offsetTop)/mapTransform.scale;\n                mapRApp.connection.invoke(\"MoveMarker\", dragItem.id, mapRApp.markers[dragItem.id].x, mapRApp.markers[dragItem.id].y);\n\n                active = false;\n            }\n        }\n\n        function drag(e) {\n            if (active) {\n                e.preventDefault();\n                if (e.type === \"touchmove\") {\n                    currentX = e.touches[0].clientX - inElementX;\n                    currentY = e.touches[0].clientY - inElementY;\n                } else {\n                    currentX = e.clientX - inElementX;\n                    currentY = e.clientY - inElementY;\n                }\n\n                setTranslate(currentX, currentY, dragItem);\n            }\n        }\n\n        function setTranslate(xPos, yPos, el) {\n            var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ xPos + ', ' + yPos + ')';\n            el.style.transform = transformValue; \n        }\n    }\n};\n\nVue.component('image-upload', {\n    data: function(){\n        return {\n            imagePreview: \"\",\n            image:null\n        };\n    },\n    methods:{\n        fileSelected: function(event){\n            let self = this;\n            self.imagePreview = \"\";\n            self.image = null;\n\n            if(event.target.files.length == 0) return;\n            self.image = event.target.files[0];\n\n            let fileReader = new FileReader();\n            fileReader.addEventListener('load', function(){\n                self.imagePreview = fileReader.result;\n            }, false);\n            fileReader.readAsDataURL(self.image);\n            this.$emit('update:image', self.image)\n        }\n    },\n    template: `\n        <div>\n            <input type=\"file\" class=\"form-input\"\n                name=\"imageUpload\" id=\"imageUpload\"\n                accept=\"image/png, image/jpeg\"\n                v-on:change=\"fileSelected\" />\n            <img id=\"imagePreview\" v-bind:src=\"imagePreview\" style=\"width:100%;\" />\n        </div>\n    `\n});\n\nVue.component('add-marker-vue', {\n    props:['gameId', 'mapId', 'connection'],\n    data: function(){\n        return {\n            markerName: \"\",\n            customCSS: \"\",\n            description: \"\",\n            nameErrorMessage: \"\",\n            formErrorMessage: \"\",\n            imageData: null\n        };\n    },\n    methods:{\n        submit: function() {\n            var self = this;\n\n            var newMarkerUrl = '/games/' + self.gameId + '/maps/' + self.mapId + '/markers/AddMarker';\n\n            let formData = new FormData();\n            console.log(self.imageData)\n            formData.append(\"ImageData\", self.imageData);\n            formData.set(\"Name\", self.markerName);\n            formData.set(\"Description\", self.description);\n            formData.set(\"CustomCSS\", self.customCSS);\n\n            const config =  { headers: {'Content-Type': 'multipart/form-data' }};\n            axios({\n                method: 'post',\n                url: newMarkerUrl,\n                data: formData,\n                config: config\n            }).then(function (response) {\n                self.formErrorMessage = \"\";\n            }).catch(function (error) {\n                self.formErrorMessage = error.message;\n            });\n\n        },\n        checkIsEmpty: function(event) {\n        },\n        emptyForm: function(){\n            this.markerName = '';\n            this.customCSS = '';\n            this.description = '';\n        }\n    },\n    template:`\n    <div class=\"modal\" role=\"form\" id=\"newMarkerModal\">\n        <div class=\"modal-dialog\" role=\"document\">\n            <div class=\"modal-content\">\n                <div class=\"modal-header\">\n                    <h5>Add New Marker</h5>\n                </div>\n                <div class=\"modal-body\" v-bind:class=\"{ 'alert-danger' : (formErrorMessage.length > 0) }\">\n                    <div class=\"form-group\">\n                        <input type=\"text\" name=\"name\" class=\"form-control\"\n                            placeholder=\"New Marker Name\"\n                            v-model=\"markerName\"\n                            v-bind:class=\"{ 'is-invalid' : (nameErrorMessage.length > 0) }\"\n                            v-on:keyup=\"checkIsEmpty\" />\n                    </div>\n\n                    <div class=\"alert alert-warning\" v-show=\"nameErrorMessage.length > 0\">{{nameErrorMessage}}</div>\n\n                    <div class=\"form-group\">\n                        <input type=\"text\"\n                            class=\"form-control\"\n                            placeholder=\"Custom CSS\"\n                            v-model=\"customCSS\" />\n                    </div>\n\n                    <div class=\"form-group\">\n                        <label for=\"\">Description</label>\n                        <textarea class=\"form-control\" rows=\"3\" v-model=\"description\"></textarea>\n                    </div>\n\n                    <image-upload v-bind:image.sync=\"imageData\"></image-upload>\n\n                    <div class=\"alert alert-warning\" v-show=\"formErrorMessage.length > 0\">{{formErrorMessage}}</div>\n                </div>\n                <div class=\"modal-footer\">\n                    <button type=\"button\" class=\"btn btn-outline-secondary\" data-dismiss=\"modal\">Close</button>\n                    <button type=\"button\" class=\"btn btn-outline-primary\" v-on:click=\"submit\">Create</button>\n                </div>\n            </div>\n        </div>\n    </div>\n    `,\n    mounted: function(){\n        global = this;\n    }\n});\n\nVue.component('add-map-vue', {\n    props:['gameId'],\n    data: function(){\n        return {\n            mapName: \"\",\n            image: null,\n            nameErrorMessage: \"\",\n            formErrorMessage: \"\"\n        };\n    },\n    methods: {\n        submit: function (event) {\n            let self = this;\n\n            if (this.checkIsEmpty()) return;\n            if (self.image == null) return;\n            var newMapUrl = '/games/' + self.gameId + '/maps/AddMap';\n\n            let formData = new FormData();\n            formData.append(\"ImageData\", self.image);\n            formData.set(\"Name\", self.mapName);\n\n            const config =  { headers: {'Content-Type': 'multipart/form-data' }};\n            axios({\n                method: 'post',\n                url: newMapUrl,\n                data: formData,\n                config: config\n            }).then(function (response) {\n                self.formErrorMessage = \"\";\n            }).catch(function (error) {\n                self.formErrorMessage = error.message;\n            });\n        },\n        checkIsEmpty: function () {\n            if (this.mapName.trim().length == 0) {\n                this.nameErrorMessage = \"Map needs a name!\";\n                return true;\n            }\n            else {\n                this.nameErrorMessage = \"\";\n                return false;\n            }\n        }\n    },\n    template:`\n    <div class=\"modal\" role=\"form\" id=\"newMapModal\">\n        <div class=\"modal-dialog\" role=\"document\">\n            <div class=\"modal-content\">\n                <div class=\"modal-header\">\n                    <h5>Add New Map</h5>\n                </div>\n                <div class=\"modal-body\" v-bind:class=\"{ 'alert-danger' : (formErrorMessage.length > 0) }\">\n                    <div class=\"form-group\">\n                        <input type=\"text\" name=\"name\" class=\"form-control\"\n                            placeholder=\"New Map Name\"\n                            v-model=\"mapName\"\n                            v-bind:class=\"{ 'is-invalid' : (nameErrorMessage.length > 0) }\"\n                            v-on:keyup=\"checkIsEmpty\" />\n                    </div>\n\n                    <div class=\"alert alert-warning\" v-show=\"nameErrorMessage.length > 0\">{{nameErrorMessage}}</div>\n\n                    <div class=\"form-group\">\n                        <label class=\"form-label\" for=\"imageUpload\">Map Image</label>\n                        <image-upload v-bind:image.sync=\"image\"></image-upload>\n                    </div>\n                    <div class=\"alert alert-warning\" v-show=\"formErrorMessage.length > 0\">{{formErrorMessage}}</div>\n                </div>\n                <div class=\"modal-footer\">\n                    <button type=\"button\" class=\"btn btn-outline-secondary\" data-dismiss=\"modal\">Close</button>\n                    <button type=\"button\" class=\"btn btn-outline-primary\" v-on:click=\"submit\">Create</button>\n                </div>\n            </div>\n        </div>\n    </div>\n    `\n});\n/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(/*! ./../../../node_modules/webpack/buildin/global.js */ \"./node_modules/webpack/buildin/global.js\")))\n\n//# sourceURL=webpack://%5Bname%5D/./Features/PlayGame/js/GameAdmin.js?");

/***/ }),

/***/ "./node_modules/webpack/buildin/global.js":
/*!***********************************!*\
  !*** (webpack)/buildin/global.js ***!
  \***********************************/
/*! no static exports found */
/***/ (function(module, exports) {

eval("var g;\n\n// This works in non-strict mode\ng = (function() {\n\treturn this;\n})();\n\ntry {\n\t// This works if eval is allowed (see CSP)\n\tg = g || new Function(\"return this\")();\n} catch (e) {\n\t// This works if the window reference is available\n\tif (typeof window === \"object\") g = window;\n}\n\n// g can still be undefined, but nothing to do about it...\n// We return undefined, instead of nothing here, so it's\n// easier to handle this case. if(!global) { ...}\n\nmodule.exports = g;\n\n\n//# sourceURL=webpack://%5Bname%5D/(webpack)/buildin/global.js?");

/***/ })

/******/ })["default"];