<template>
  <div class="stats">
    <hr />

    <div>
      <div>
        Enemies Killed:
        <span class="negative">{{ enemiesKilled }}</span>
      </div>

      <div>
        Healthpacks Picked Up:
        <span class="positive">{{ healthPacksPickedUp }}</span>
      </div>
    </div>

    <hr />
  </div>
</template>

<script lang="js">
import { defineComponent, toRefs } from "vue";

import { useState } from "@/shared/hooks/use-state";

export default defineComponent({
  setup() {
    const { state } = useState();

    return {
      ...toRefs(state),
    };
  },
});
</script>

<style lang="scss">
@import "@/shared/scss/base.scss";

.stats {
  margin-bottom: 2rem;

  div {
    font-weight: bold;
    font-size: 1.2rem;
  }
}
</style>
