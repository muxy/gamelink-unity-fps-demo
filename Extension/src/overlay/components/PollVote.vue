<template>
  <div class="voting" :class="{ voted: countdownTimer === 0 }">
    <div v-if="!voted" class="instructions">
      Vote to change the gravity!

      <div class="timer">
        Time Left To Vote:
        <div class="clock">{{ countdownTimer }}</div>
      </div>
    </div>

    <div v-else class="instructions">Your vote has been counted!</div>

    <div class="actions" v-if="!voted">
      <button :disabled="countdownTimer <= 0" @click="voteForOption(0)">
        Low Gravity
      </button>

      <button :disabled="countdownTimer <= 0" @click="voteForOption(1)">
        High Gravity
      </button>
    </div>
  </div>
</template>

<script lang="js">
import { defineComponent, onMounted, ref } from "vue";

import { useMEDKit } from "@/shared/hooks/use-medkit";

export default defineComponent({
  props: {
    eventDuration: {
      default: 25,
      type: Number,
    },
  },

  setup(props) {
    const { medkit } = useMEDKit();
    const voted = ref(false);

    const countdownTimer = ref(props.eventDuration);

    const voteForOption = (option) => {
      medkit.vote("gravityMode", option).then(() => {
        voted.value = true;
        countdownTimer.value = 3;
      });
    };

    const startCountdown = () => {
      if (countdownTimer.value > 0) {
        setTimeout(() => {
          countdownTimer.value -= 1;
          startCountdown();
        }, 1000);
      }
    };

    onMounted(() => {
      startCountdown();
    });

    return {
      voteForOption,
      countdownTimer,
    };
  },
});
</script>

<style lang="scss">
@import "@/shared/scss/base.scss";

.voting {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;

  background-color: rgba(0, 0, 0, 0.9);

  width: 100vw;
  height: 100vh;

  position: absolute;
  top: 0;

  &.voted {
    display: none;
  }

  .instructions {
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;

    color: #fff;

    .timer {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      margin: 16px 0;

      .clock {
        display: flex;
        align-items: center;
        color: yellow;
        font-size: 1.4em;
      }
    }
  }

  .actions {
    display: flex;
    justify-content: center;
    align-items: center;
  }
}
</style>
