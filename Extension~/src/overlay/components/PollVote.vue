<template>
  <div class="voting">
    <div class="instructions">
      Vote to change the gravity!

      <div class="timer">
        Time Left To Vote:
        <div class="clock">{{ countdownTimer }}</div>
      </div>
    </div>

    <div class="actions">
      <button :disabled="countdownTimer <= 0" @click="voteForOption(0)">
        Low Gravity
      </button>

      <button :disabled="countdownTimer <= 0" @click="voteForOption(1)">
        High Gravity
      </button>
    </div>
  </div>
</template>

<script lang="ts">
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

    const countdownTimer = ref(props.eventDuration);

    const voteForOption = (option: number) => {
      medkit.vote("gravityMode", option);
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
@import "~@/shared/scss/base.scss";

.voting {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;

  .instructions {
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;

    .timer {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      margin: 16px 0;

      .clock {
        display: flex;
        align-items: center;
        color: $log-yellow;
        font-size: 1.4em;
      }
    }
  }

  .actions {
    button {
      &:first-of-type {
        margin-right: 8px;
      }
    }
  }
}
</style>
