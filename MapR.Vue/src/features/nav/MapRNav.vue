<template>
    <nav class="navbar">
        <div class="nav-wrapper">
            <ul>
                <li>
                    <router-link to="/">Home</router-link>
                </li>
                <li class="right" v-if="!userInfo.loadedUserInfo || userInfo.user === ''">
                    <a v-bind:href="functionsUrl+'api/login/google?redirect=' + appServerUrl">google</a>
                </li>
                <li class="right" v-if="userInfo.loadedUserInfo && userInfo.user !== ''">
                    <a v-bind:href="functionsUrl+'api/logout'">logout</a>
                </li>
                <li class="right">
                    <span v-if="userInfo.loadedUserInfo && userInfo.user !== ''">
                        {{userInfo.user}}
                    </span>
                    <span v-else-if="userInfo.loadedUserInfo && userInfo.user === ''">
                        Login with a provider
                    </span>
                    <span v-else>loading user information</span>
                </li>
            </ul>
        </div>
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
<style lang="scss" scoped>
    @import '../shared/variables.scss';
    .navbar{
        position:fixed;
        width:100%;
        top:0px;
        height:$menu-height + 'px';
        .nav-wrapper{
            padding-top: 0px;
            background-color:$papyrus;
            ul {
                margin:0px;
                list-style: none;
                li {
                    display: inline-block;
                    * {
                        -webkit-transition: background-color .3s;
                        transition: background-color .3s;
                        font-size: 1rem;
                        color: $link-color;
                        display: block;
                        padding:15px;
                        text-decoration: none; 
                    }
                    a:hover{
                        background-color:$papyrus-dark;
                        cursor: pointer;
                    }
                    span {
                        color: black;
                    }
                }
            }
        }
    }
</style>