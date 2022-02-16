<template>
  <div class="config">
    <div class="title">Configuration Pin</div>
    <GameAuth />
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";

import { isDebugUser } from "@/shared/debug";
import globals from "@/shared/globals";

import { provideMEDKit } from "@/shared/hooks/use-medkit";

import GameAuth from "./components/GameAuth.vue";

export default defineComponent({
  name: "App",
  components: { GameAuth },

  setup() {
    const medkit = provideMEDKit({
      channelId: globals.TESTING_CHANNEL_ID,
      clientId: globals.CLIENT_ID,
      role: "broadcaster",
      uaString: globals.UA_STRING,
      userId: globals.TESTING_USER_ID,
    });

    const isDebugging = ref(false);
    const testingTimeout = ref(0);

    medkit.loaded().then(() => {
      isDebugging.value = isDebugUser(medkit.user.twitchID);
    });

    return {
      isDebugging,
    };
  },
});
</script>

<style lang="scss">
@import "~@/shared/scss/base.scss";
@import "~@/shared/scss/colors.scss";
@import "~@/shared/scss/utils.scss";

#app {
  display: flex;
  justify-content: center;
  background-color: #000;
  padding: 24px;

  color: $log-white;

  .config {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;

    width: 50%;
    background-color: rgba($log-purple, 0.5);
    border: 1px solid $log-purple;
    border-radius: 4px;
    padding: 24px;

    .title {
      border-bottom: 1px solid #666;
      margin: 0.5em 0 1em 0;
      padding-bottom: 0.5em;

      color: $log-pink;
      font-weight: bold;
      font-size: 1.4em;
      opacity: 0.6;
    }

    .debugger {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;

      h1 {
        font-size: 0.8em;
        color: $log-yellow;
      }

      .actions {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;

        button {
          margin-bottom: 8px;
        }
      }
    }
  }
}
</style>
