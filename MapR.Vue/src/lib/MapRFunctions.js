import axios from 'axios';

const config = require('../../config.json')

export default {
   async getUser(){
       return await axios.get(config.mapRFunctionsUrl + 'api/user');
   } 
}