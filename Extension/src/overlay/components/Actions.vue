<template>
  <div class="actions">
    <h4 class="positive">Help The Player!</h4>

    <button @click="spawnPickup('healthpack')">Spawn Healthpack</button>
    <button @click="spawnPickup('shotgun')">Spawn Shotgun</button>
    <button @click="spawnPickup('jetpack')">Spawn Jetpack</button>
    <button @click="spawnPickup('launcher')">Spawn Launcher</button>

    <h4 class="negative">Sabotage The Player!</h4>

    <button @click="spawnMonster('hoverbot')">Spawn Hoverbot</button>
    <button @click="spawnMonster('turret')">Spawn Turret</button>

    <BitsInterface />
  </div>
</template>

<script lang="js">
import { defineComponent } from "vue";

import { useMEDKit } from "@/shared/hooks/use-medkit";
import { useTwitchContext } from "@/shared/hooks/use-twitchcontext";

import BitsInterface from "@/overlay/components/BitsInterface.vue";

export default defineComponent({
  components: {
    BitsInterface,
  },

  setup() {
    const { medkit } = useMEDKit();

    const sendDatastream = (event) => {
      medkit.signedRequest("POST", "datastream", event);
    };

    const spawnMonster = (monsterType) => {
      var event = {
        spawnMonsterType: monsterType,
        spawnPickupType: "",
      };
      sendDatastream(event);
    };

    const spawnPickup = (pickupType) => {
      var event = {
        spawnMonsterType: "",
        spawnPickupType: pickupType,
      };
      sendDatastream(event);
    };

    return {
      spawnMonster,
      spawnPickup,
    };
  },
});
</script>

<style lang="scss">
@import "@/shared/scss/base.scss";

.actions {
  display: flex;
  flex-direction: column;

  background-color: #000;
  border: 1px solid #fff;
  padding: 1.2rem;

  h4 {
    &:first-of-type {
      margin-top: 0;
    }
  }

  button {
    margin-bottom: 0.5rem;
  }
}
</style>
