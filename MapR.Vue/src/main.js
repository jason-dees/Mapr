import Vue from 'vue'
import VueRouter from 'vue-router'
import Index from './features/index/Index.vue'
import Game from './features/game/Game.vue'
import Games from './features/games/Games.vue'

Vue.config.productionTip = false

Vue.use(VueRouter);
const routes = [
  { path: '/', component: Games, props: true },
  { path: '/games/:id', component: Game, props: true },
  { path: '/games', component: Games, props: true },
];
const router = new VueRouter({
  routes // short for `routes: routes`
});
new Vue({
  render: h => h(Index),
  router,
}).$mount('#app')
