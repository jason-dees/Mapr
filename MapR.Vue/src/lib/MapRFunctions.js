import axios from 'axios';
import config from '../../config.json';

const getInstance = function(){
    return axios.create({
        baseURL: config.mapRFunctionsUrl + 'api/',
        withCredentials: true
    });
}

export default {
    async getUser(){
       return await getInstance().get('user');
    }, 
    async getGames(){
       return await getInstance().get('/games');
    },
    async getGame(gameId){
       return await getInstance().get('games/' + gameId);
    },
    async negotiateSignalr(){
       return await getInstance().get('negotiate');
    },
    async addToGame(gameId){
        return await getInstance().post('AddToGame', {gameId});
    }
}