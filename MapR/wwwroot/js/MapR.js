"use strict";

var mapr = function() {
    mapRApp = new Vue({
        el: '#MapR',
        data: {
            mapImage: "~/images/maps/SKT/Campaign.jpg",
            mapMarkers: {},
        },
        updated: function(){
            this.$nextTick(function () {
                resetMapMarkers();
              })
        }
    });
    var mapZoom = panzoom(document.querySelector('#map'), {
        maxZoom: 1,
        smoothScroll: false,
        minZoom: .1
    });

    mapZoom.on('transform', resetMapMarkers);

    function resetMapMarkers() {
        for (var marker in mapRApp.mapMarkers) {
            setMapMarker(marker);
        }
    }
    function addMarker(marker){
        mapRApp.$set(mapRApp.mapMarkers, marker.id, marker);
    }

    function getMarker(id){
        return mapRApp.mapMarkers[id];
    }

    function setMapMarker(mapMarker) {
        addMarker(mapMarker);

        var mapTransform = mapZoom.getTransform();
        var element = document.querySelector('#' + mapMarker.id);

        var mapMarkerX = mapMarker.x - element.width() / 2,
            mapMarkerY = mapMarker.y - element.height() / 2,
        	left = mapTransform.scale * mapMarkerX + mapTransform.x,
			top = mapTransform.scale * mapMarkerY + mapTransform.y;

        var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ left + ', ' + top + ')';
        element.css('transform', transformValue);
    }

    var connection = new signalR.HubConnectionBuilder().withUrl("/mapHub").build();

    connection.on("SetMapMarker", setMapMarker);

    connection.on("SetAllMapMarkers", function (mapMarkers) {
        mapMarkers.forEach(addMarker);
    });

    $('#party').on('click', function () {
        var mapMarker = { id: 'party', x: 100, y: 100 };
        connection.invoke('SendMapMarker', mapMarker);
    });

    $('#center').on('click', function(e){
        var partyMarker = getMarker('party'); 
        mapZoom.moveTo(partyMarker.x, partyMarker.y, 1);
        e.preventDefault();
    });

    connection.start()
        .then(function () { connection.invoke("SendAllMapMarkers") });
}

window.onload = mapr; 