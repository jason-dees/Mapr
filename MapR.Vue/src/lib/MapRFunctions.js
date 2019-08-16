import axios from 'axios';
import config from '../../config.json';
import { store } from '../features/shared/store.js'

const axiosOptions = {  };
const getInstance = function(){
    console.log(store.state.user);
    return axios.create({
        baseURL: config.mapRFunctionsUrl + 'api/',
        withCredentials: true
    });
}

export default {
    async getUser(){
        console.log("get user")
       return await getInstance().get('user');
    }, 
    async getGames(){
       return await axios.get(config.mapRFunctionsUrl + 'api/games', axiosOptions);
    },
    async getGame(gameId){
        console.log("getgame")
       return await getInstance().get('games/' + gameId);
    },
    async negotiateSignalr(){
        console.log("neg signal")
       return await getInstance().get('negotiate');
    },
    async addToGame(gameId){
        return await getInstance().post('AddToGame', {gameId});
    }
}