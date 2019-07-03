<template>
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <router-link to="/">Home</router-link>
        <a v-bind:href="functionsUrl+'api/login/google?redirect=' + appServerUrl">google</a>
        <span v-if="userInfo.loadedUserInfo && userInfo.user !== ''">
            {{userInfo.user}}
        </span>
        <span v-else-if="userInfo.loadedUserInfo && userInfo.user === ''">
            I need to do javascript app auth with google or whomever
        </span>
        <span v-else>loading user information</span>
        <a v-bind:href="functionsUrl+'api/logout'">logout</a>
    </nav> 
</template>
<script>
export default{
    name: 'MapRNav',
    props: {
        userInfo: Object,
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