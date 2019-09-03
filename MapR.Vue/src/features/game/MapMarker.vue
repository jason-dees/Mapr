<template>
    <div v-bind:style="computedCss" v-bind:id="marker.Id" class="marker">
        </div>
</template>

<script>
import config from '../../../config.json';

export default {
    name: 'map-marker',
    props: ['marker'],
    computed: {
        computedCss: function(){
            var url = `${config.mapRFunctionsUrl}api/games/${this.marker.GameId}/maps/${this.marker.MapId}/markers/${this.marker.Id}/image`;
            var backgroundImage = 'background-image: url("' + url + '?width=25&height=25");';
            return backgroundImage + this.marker.customCSS;
        }
    },
    mounted:function(){
        // $('#' + this.marker.id).popover({
        //     container:'body',
        //     content: this.marker.description || "",
        //     title: this.marker.name
        // });
    }
}
</script>