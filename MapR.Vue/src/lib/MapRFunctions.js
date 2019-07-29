import axios from 'axios';

const config = require('../../config.json')

export default {
    async getUser(){
       return await axios.get(config.mapRFunctionsUrl + 'api/user', {withCredentials: true});
    }, 
    async getGames(){
       return await axios.get(config.mapRFunctionsUrl + 'api/games', {withCredentials: true});
    },
    async getGame(gameId){
       return await axios.get(config.mapRFunctionsUrl + 'api/games/' + gameId, {withCredentials: true});
    },
    async negotiateSignalr(){
       return await axios.get(config.mapRFunctionsUrl + 'api/negotiate', {withCredentials: true});
    },
    async addToGame(gameId){
        return await axios.post(config.mapRFunctionsUrl + 'api/AddToGame', {gameId}, {withCredentials: true});
    }
}