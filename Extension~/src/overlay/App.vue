<template>
  <h2>Enemies Killed: {{ state.enemiesKilled }}</h2>
  <h2>Healthpacks Picked Up: {{ state.healthpacksPickedUp }}</h2>
  <div>
    <h4 style="color: green">Help The Player!</h4>
    <button @click="spawnPickup('healthpack')">Spawn Healthpack</button>
    <button @click="spawnPickup('shotgun')">Spawn Shotgun</button>
    <button @click="spawnPickup('jetpack')">Spawn Jetpack</button>
    <button @click="spawnPickup('launcher')">Spawn Launcher</button>
    <h4 style="color: red">Sabotage The Player!</h4>
    <button @click="spawnMonster('hoverbot')">Spawn Hoverbot</button>
    <button @click="spawnMonster('turret')">Spawn Turret</button>
    <button @click="spawnWithBits('turret')">Spawn Turret for Bits</button>
  </div>

  <PollVote v-if="isVoting" />
</template>

<script lang="ts">
import { defineComponent, reactive, ref } from "vue";

import globals from "@/shared/globals";

import { ChannelState, DatastreamEvent } from "@/shared/types/state";

import { provideMEDKit } from "@/shared/hooks/use-medkit";
import PollVote from "@/overlay/components/PollVote.vue";
import {
  useTwitchContext,
  onBitsTransaction,
} from "@/shared/hooks/use-twitchcontext";

export default defineComponent({
  name: "App",
  components: {
    PollVote,
  },

  setup() {
    const medkit = provideMEDKit({
      channelId: globals.TESTING_CHANNEL_ID,
      clientId: globals.CLIENT_ID,
      role: "viewer",
      uaString: globals.UA_STRING,
      userId: globals.TESTING_USER_ID,
    });

    useTwitchContext();

    // check channel state for active event
    const state = reactive<ChannelState>({
      enemiesKilled: 0,
      healthpacksPickedUp: 0,
    });

    const sendDatastream = (event: DatastreamEvent) => {
      medkit.loaded().then(() => {
        medkit.signedRequest("POST", "datastream", JSON.stringify(event));
      });
    };

    const spawnMonster = (monsterType: string) => {
      var event: DatastreamEvent = {
        spawnMonsterType: monsterType,
        spawnPickupType: "",
      };
      sendDatastream(event);
    };

    const spawnPickup = (pickupType: string) => {
      var event: DatastreamEvent = {
        spawnMonsterType: "",
        spawnPickupType: pickupType,
      };
      sendDatastream(event);
    };

    const updateState = async () => {
      const newState = (await medkit.getChannelState()) as ChannelState;
      if (newState.enemiesKilled) {
        state.enemiesKilled = newState.enemiesKilled;
        state.healthpacksPickedUp = newState.healthpacksPickedUp;
      }
    };

    onBitsTransaction(async (tx) => {
      if (tx.initiator.toUpperCase() === "CURRENT_USER") {
        console.log("onBitsTransaction");
      }
    });

    const isVoting = ref(false);
    medkit.loaded().then(async () => {
      updateState();

      // listen for new event availability
      medkit.listen("start_poll", () => {
        isVoting.value = true;
      });

      medkit.listen("stop_poll", () => {
        isVoting.value = false;
      });

      medkit.listen("channel_state_update", async () => {
        updateState();
      });

      medkit.listen("game_over", () => {
        isVoting.value = false; // hide voting
        updateState();
      });
    });

    return {
      isVoting,
      state,
      spawnPickup,
      spawnMonster,
    };
  },
});
</script>

<style lang="scss">
@import "~@/shared/scss/base.scss";
body {
  background-color: #101010;
}
button {
  margin-bottom: 8px;
  margin-left: 12px;
}
</style>
