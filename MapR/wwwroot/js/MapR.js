function setMarker(marker, map) {
    var mapTransform = map.getTransform();
    var element = document.querySelector('#' + marker.id);
    var mapElement = document.querySelector('#map');
    var markerX = marker.x,
        markerY = marker.y,
        left = mapTransform.scale * markerX + mapTransform.x + mapElement.offsetLeft,
        top = mapTransform.scale * markerY + mapTransform.y + mapElement.offsetTop;

    var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ left + ', ' + top + ')';
    element.style.transform = transformValue;
}

function resetMapMarkers(markers, map) {
    for (var marker in markers) {
        setMarker(markers[marker], map);
    }
}