<template>
    <div v-bind:style="computedCss" v-bind:id="marker.Id" class="marker">
        </div>
</template>

<script>
export default {
    name: 'marker',
    props: ['marker'],
    computed: {
        computedCss: function(){
            var backgroundImage = 'background-image: url("' + this.marker.IconUrl + '?width=25");';
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