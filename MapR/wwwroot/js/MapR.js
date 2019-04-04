"use strict";

var mapr = function() {
    var mapMarkers = {};
    var mapZoom = panzoom(document.querySelector('#map'), {
        maxZoom: 1,
        smoothScroll: false,
        minZoom: .1
    });

    mapZoom.on('pan', resetMapMarkers);
    mapZoom.on('zoom', resetMapMarkers);

    function resetMapMarkers() {
        for (var marker in mapMarkers) {
            setMapMarker(mapMarkers[marker]);
        }
    }

    function setMapMarker(mapMarker) {
        mapMarkers[mapMarker.id] = mapMarker
        var mapTransform = mapZoom.getTransform();
        var element = $(mapMarker.id);
        //Try a stepping for the scale rather than just the decimal number
        var mapMarkerX = mapMarker.x - element.width() / 2,
            mapMarkerY = mapMarker.y - element.height() / 2;
        var left = mapTransform.scale * mapMarkerX + mapTransform.x;
        var top = mapTransform.scale * mapMarkerY + mapTransform.y;
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


    connection.start()
        .then(function () { connection.invoke("SendAllMapMarkers") });
}

window.onload = mapr; 