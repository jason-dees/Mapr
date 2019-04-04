"use strict";

var global;
var mapr = function() {
    var mapMarkers = {};
    var mapZoom = panzoom(document.querySelector('#map'), {
        maxZoom: 1,
        smoothScroll: false,
        minZoom: .1
    });

    mapZoom.on('transform', resetMapMarkers);

    function resetMapMarkers(e) {
        global = e;
        for (var marker in mapMarkers) {
            setMapMarker(mapMarkers[marker], e);
        }
    }

    function setMapMarker(mapMarker) {
        mapMarkers[mapMarker.id] = mapMarker

        var mapTransform = mapZoom.getTransform();
        var element = $(mapMarker.id);

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
        mapMarkers.forEach(setMapMarker);
    });

    $('#party').on('click', function () {
        var mapMarker = { id: '#party', x: 100, y: 100 };
        connection.invoke('SendMapMarker', mapMarker);
    });

    $('#center').on('click', function(e){
        var partyMarker = mapMarkers['#party']; 
        mapZoom.moveTo(partyMarker.x, partyMarker.y, 1);
        e.preventDefault();
    });

    connection.start()
        .then(function () { connection.invoke("SendAllMapMarkers") });
}

window.onload = mapr; 