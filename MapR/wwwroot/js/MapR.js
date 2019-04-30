function setMarker(marker, mapZoom, mapElement) {
    var mapTransform = mapZoom.getTransform();
    var element = document.querySelector('#' + marker.id);
    var markerX = marker.x,
        markerY = marker.y,
        left = mapTransform.scale * markerX + mapTransform.x + mapElement.offsetLeft,
        top = mapTransform.scale * markerY + mapTransform.y + mapElement.offsetTop;

    var transformValue = 'matrix(' + mapTransform.scale + ',0, 0, ' + mapTransform.scale + ', '+ left + ', ' + top + ')';
    element.style.transform = transformValue;
}

function resetMapMarkers(markers, map, mapElement) {
    for (var marker in markers) {
        setMarker(markers[marker], map, mapElement);
    }
}
//Do a MapVue Component here with display of markers and map

Vue.component('map-vue', {
    props:['mapSrc', 'markers'],
    data: function(){
        return {

        };
    },
    template:`
        <div>
            <div class="mapContainer">
                <img class="map" v-bind:src="mapSrc" style="min-width:100px;min-height:100px"/>
            </div>
            <map-marker-vue 
                v-for="marker in markers"
                v-bind:key="marker.id"
                v-bind:marker="marker">
            </map-marker-vue>
        </div>  
    `,
    mounted: function(){
    }
});

Vue.component('map-marker-vue', {
    props:['marker'],
    data: function(){
        return {
        };
    },
    computed: {
        computedCss: function(){
            var backgroundImage = 'background-image: url("' + this.marker.iconUrl + '?width=25");';
            return backgroundImage + this.marker.customCSS;
        }
    },
    mounted:function(){
        $('#' + this.marker.id).popover({
            container:'body',
            content: this.marker.description || "",
            title: this.marker.name
        });
    },
    template:`
        <div v-bind:style="computedCss" v-bind:id="marker.id" class="marker">
        </div>
    `
});