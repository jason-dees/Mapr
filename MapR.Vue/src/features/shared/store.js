import mapRFunctions from '../../lib/MapRFunctions.js'

var store = {
    state: {
        title: 'MapR',
        user: '',
        loadedUserInfo: false,
        loadingUserInfo: true,
        connection: null,
        game:{
            markers: {},
            isOwner: false
        }
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
    addToGame(gameId){
        return mapRFunctions.addToGame(gameId);
    },
    setIsGameOwner(isGameOwner){
        this.state.game.isOwner = isGameOwner;
    },
    addMarker(marker){
        this.state.game.markers[marker.Id] = marker;
        console.log(this.state.game.markers);
    },
    resetGame(){
        this.state.game = {
            markers: {},
            isOwner: false
        };
    },
    clearMarkers(){
        this.state.game.markers = {};
    }
};
export {store}