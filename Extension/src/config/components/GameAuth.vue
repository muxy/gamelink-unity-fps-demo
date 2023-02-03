<template>
  <div class="game-auth-container">
    <template v-if="pin">
      <div class="pin">
        PIN:
        <span>{{ pin || "No PIN found" }}</span>
      </div>
    </template>
  </div>
</template>

<script lang="js">
import { defineComponent, onMounted, ref } from "vue";

import { useMEDKit } from "@/shared/hooks/use-medkit";

export default defineComponent({
  name: "GameAuth",

  setup() {
    const { medkit } = useMEDKit();

    const loaded = ref(false);
    const pin = ref("");
    const configErr = ref("");

    async function requestPIN() {
      if (!medkit) {
        configErr.value = "MEDKit could not be initialized correctly.";
      }

      try {
        await medkit.loaded();
        const resp = await medkit.signedRequest(
          "POST",
          "gamelink/token",
          JSON.stringify({})
        );

        pin.value = resp.token;
      } catch (err) {
        configErr.value = "Could not get PIN from server";
      }
    }

    onMounted(() => {
      requestPIN();
      loaded.value = true;
    });

    return {
      loaded,
      pin,
      requestPIN
    };
  }
});
</script>

<style lang="scss" scoped>
@import "@/shared/scss/base.scss";

.game-auth-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 0;

  .pin {
    display: flex;
    justify-content: center;
    align-items: center;

    background: #000;
    padding: 10px;

    color: #fff;

    span {
      margin-left: 20px;
      font-size: 2rem;
      line-height: 1em;
      // text-shadow: 2px 2px 0px rgba(0, 0, 0, 20%);
      text-shadow: none;
    }
  }
}
</style>
