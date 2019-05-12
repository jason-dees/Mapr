var gamePlayer =
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
/******/ 	return __webpack_require__(__webpack_require__.s = "./Features/PlayGame/js/GamePlayer.js");
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

/***/ "./Features/PlayGame/js/GamePlayer.js":
/*!********************************************!*\
  !*** ./Features/PlayGame/js/GamePlayer.js ***!
  \********************************************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony import */ var _Game__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./Game */ \"./Features/PlayGame/js/Game.js\");\n\n\nvar gamePlayer = function (gameId) {\n    var vue = Object(_Game__WEBPACK_IMPORTED_MODULE_0__[\"addToGame\"])(gameId);\n};\n\n//# sourceURL=webpack://%5Bname%5D/./Features/PlayGame/js/GamePlayer.js?");

/***/ })

/******/ })["default"];