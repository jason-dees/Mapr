import Axios from 'axios';

export default {
   getUser: function(){
        return Axios.get('https://maprfunctions.azurewebsites.net/api/user', {withCredentials: true});
   }
}