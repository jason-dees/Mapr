<template>
    <nav class="navbar">
        <div class="nav-wrapper">
            <ul class="left">
                <li>
                    <router-link to="/">Home</router-link>
                </li>
            </ul>
            <span class="title">
                {{sharedState.title}}
            </span>
            <ul class="right" v-if="!sharedState.loadedUserInfo && !sharedState.loadingUserInfo">
                <li>
                    <span>
                        Login with a provider
                    </span>
                </li>
                <li>
                    <a  href="#" v-on:click.prevent="googleLogin">google</a>
                </li>
            </ul>
            <span class="right" v-else-if="sharedState.loadingUserInfo">Loading User</span>
            <ul class="right" v-else>
                <li>
                    <span v-if="sharedState.loadedUserInfo">
                        {{sharedState.user}}
                    </span>
                </li>
                <li>
                    <a v-bind:href="functionsUrl+'api/logout'">logout</a>
                </li>
            </ul>
        </div>
    </nav> 
</template>
<script>
import {store} from '../shared/store.js'
export default{
    name: 'MapRNav',
    props: {
        functionsUrl: String,
        appServerUrl: String
    },
    data: function(){
        return {
            sharedState: store.state,
            googleUrl: `${this.functionsUrl}.auth/login/google?post_login_redirect_url=${encodeURIComponent(window.location.href)}`
        };
    },
    methods: {
        googleLogin: function(){
            window.location.href = this.googleUrl;
        }
    }
}
</script>
<style lang="scss" scoped>
    @import '../shared/variables.scss';
    .navbar{
        position:fixed;
        width:100%;
        top:0px;
        z-index:1000;
        position:fixed;
        width:100%;
        .nav-wrapper{
            padding-top: 0px;
            background-color:$papyrus;
            height:$menu-height;
            text-align: center;
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