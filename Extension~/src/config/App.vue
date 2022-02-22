<template>
  <div class="config">
    <h1>Configuration PIN</h1>

    <GameAuth v-if="loaded" />
  </div>
</template>

<script lang="js">
import { defineComponent, ref } from "vue";

import { provideMEDKit } from "@/shared/hooks/use-medkit";

import globals from "@/shared/globals";

import GameAuth from "@/config/components/GameAuth.vue";

export default defineComponent({
  components: {
    GameAuth
  },

  setup() {
    // MEDKit is initialized and provided to the Vue provide/inject system
    const medkit = provideMEDKit({
      channelId: globals.TESTING_CHANNEL_ID,
      clientId: globals.CLIENT_ID,
      role: "broadcaster",
      uaString: globals.UA_STRING,
      userId: globals.TESTING_USER_ID,
    });

    const loaded = ref(false);

    medkit.loaded().then(() => {
      loaded.value = true;
    });

    return {
      loaded,
    }
  },
});
</script>

<style lang="scss">
@import "@/shared/scss/base.scss";

.config {
  display: flex;
  flex-direction: column;

  height: 100vh;
  width: 100vw;

  padding: 1rem;
}
</style>
