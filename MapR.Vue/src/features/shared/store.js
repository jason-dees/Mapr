import mapRFunctions from '../../lib/MapRFunctions.js'
import * as signalR from '@aspnet/signalr';
import config from '../../../config.json';

var store = {
    state: {
        title: 'MapR',
        user: '',
        loadedUserInfo: false,
        loadingUserInfo: true,
        connection: null
    },
    setPageTitle  (newTitle){
        this.state.title = newTitle;
    },
    setUser (newUser){
        this.state.user = newUser;
    },
    getUser(){
        var self = this;
        mapRFunctions.getUser().then((r) => {
            self.setUser(r.data.name);
            self.state.loadedUserInfo = true;
        }).catch(() => {
            self.state.user = null;
            self.state.loadedUserInfo = false;
        }).finally(() => {
            self.state.loadingUserInfo = false;
        });
    },
    getSignalRConnection(){
        var self = this;
        if(self.state.connection == null){
            self.state.connection = new signalR.HubConnectionBuilder()
            .withUrl(config.mapRFunctionsUrl +'api')
            .build();

        }
        return self.state.connection;
    }
};
export {store}