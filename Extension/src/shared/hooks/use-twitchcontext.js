import { reactive, ref, toRefs } from "vue";

let loaded = false;

const transactionCallbacks = [];

export function onBitsTransaction(cb) {
  transactionCallbacks.push(cb);
}

export function useTwitchContext(medkit) {
  const state = reactive({
    arePlayerControlsVisible: false,
    bitsEnabled: false,
    requestIdShare,
  });

  const bitsEnabled = ref(window?.Twitch?.ext?.features?.isBitsEnabled);

  if (!loaded) {
    if (window.Twitch) {
      medkit.onContextUpdate((ctx) => {
        state.arePlayerControlsVisible = ctx.arePlayerControlsVisible;
      });

      window.Twitch.ext.features.onChanged(() => {
        bitsEnabled.value = window.Twitch.ext.features.isBitsEnabled;
      });

      window.Twitch.ext.bits.onTransactionComplete((tx) => {
        transactionCallbacks.forEach((cb) => cb(tx));
      });
    }

    loaded = true;
  }

  const requestIdShare = () => {
    if (window.Twitch) {
      window.Twitch.ext.actions.requestIdShare();
    }
  };

  return {
    ...toRefs(state),
  };
}
