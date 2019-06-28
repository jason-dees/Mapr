<template>
  <div>
    <img alt="Vue logo" src="../../assets/logo.png">
    this is where the active game is {{id}}
  </div>
</template>

<script>

export default {
  name: 'game',
  props:{
    id: String
  },
  components: {
  }
}
</script>
