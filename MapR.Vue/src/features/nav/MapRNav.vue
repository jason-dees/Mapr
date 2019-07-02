<template>
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <router-link to="/">Home</router-link>
        <a v-bind:href="functionsUrl+'api/login/google?redirect=' + appServerUrl">google</a>
        <a v-bind:href="functionsUrl+'api/logout'">logout</a>
    </nav> 
</template>
<script>
export default{
    name: 'MapRNav',
    props: {
        functionsUrl: String,
        appServerUrl: String
    }
}
</script>
<style>
    .navbar{
        background-color:green;
        padding:20px;
    }
</style>