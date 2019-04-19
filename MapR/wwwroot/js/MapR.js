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

//Do a MapVue Component here with display of markers and map

Vue.component('map-vue', {
    data: function(){
        return {

        };
    },
    template:`
        <div>
            <div class="mapContainer">
                <img class="map" v-bind:src="activeMap" style="min-width:100px;min-height:100px"/>
            </div>
            <map-marker-vue></map-marker-vue>
        </div>
    `
});

Vue.component('map-marker-vue', {
    data: function(){
        return {};
    },
    template:``
});