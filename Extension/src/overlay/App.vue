<template>
  <div class="overlay">
    <h1>Unity FPS Demo Extension</h1>

    <div v-if="loaded">
      <Stats />

      <div>
        <Actions />
      </div>
    </div>

    <div v-else>Loading...</div>
  </div>

  <PollVote v-if="isVoting" :eventDuration="eventDuration" />
</template>

<script lang="js">
import { defineComponent, ref } from "vue";

import { provideMEDKit } from "@/shared/hooks/use-medkit";
import { provideState } from "@/shared/hooks/use-state";

import globals from "@/shared/globals";

import Actions from "@/overlay/components/Actions.vue";
import PollVote from "@/overlay/components/PollVote.vue";
import Stats from "@/overlay/components/Stats.vue";

export default defineComponent({
  components: {
    Actions,
    PollVote,
    Stats,
  },

  setup() {
    // Initialize variables used in HTML
    const eventDuration = ref(0);
    const isVoting = ref(false);
    const loaded = ref(false);

    // MEDKit is initialized and provided to the Vue provide/inject system
    const medkit = provideMEDKit({
      channelId: globals.TESTING_CHANNEL_ID,
      clientId: globals.CLIENT_ID,
      role: "viewer",
      uaString: globals.UA_STRING,
      userId: globals.TESTING_USER_ID,
      transactionsEnabled: true,
    });

    provideState(medkit);

    // MEDKit must fully load before it is available
    medkit.loaded().then(() => {
      loaded.value = true;

      medkit.listen("start_poll", (data) => {
        if (data) {
          eventDuration.value = data.poll_duration;
        }
        isVoting.value = true;
      });

      medkit.listen("stop_poll", () => {
        isVoting.value = false;
      });

      medkit.listen("game_over", () => {
        isVoting.value = false;
      });
    });

    return {
      eventDuration,
      isVoting,
      loaded,
    };
  },
});
</script>

<style lang="scss" scoped>
@import "@/shared/scss/base.scss";

.overlay {
  display: flex;
  flex-direction: column;

  height: 100vh;
  width: 33vw;

  background-color: rgba(0, 0, 0, 0.75);
  color: white;
  padding: 1em;
}
</style>
