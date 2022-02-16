import {
  TwitchBitsProduct,
  TwitchBitsTransaction,
} from "@muxy/extensions-js/dist/types/src/twitch";
import { reactive, Ref, toRefs } from "vue";

interface TwitchContext {
  arePlayerControlsVisible: boolean;
}

const transactionCallbacks: Array<(tx: TwitchBitsTransaction) => void> = [];

export function onBitsTransaction(
  cb: (tx: TwitchBitsTransaction) => void
): void {
  transactionCallbacks.push(cb);
}

export function getProducts(): Promise<TwitchBitsProduct[]> {
  const target = window.Twitch?.ext?.bits;
  if (target) {
    return target.getProducts();
  }

  return Promise.resolve([]);
}

export function useTwitchContext(): {
  arePlayerControlsVisible: Ref<boolean>;
  isHighlighted: Ref<boolean>;
} {
  const state = reactive({
    arePlayerControlsVisible: true,
    isHighlighted: false,
  });

  if (window.Twitch) {
    window.Twitch.ext.onContext((ctx: TwitchContext) => {
      state.arePlayerControlsVisible = ctx.arePlayerControlsVisible;
    });

    window.Twitch.ext.onHighlightChanged((isHighlighted: boolean) => {
      state.isHighlighted = isHighlighted;
    });

    window.Twitch.ext.bits.onTransactionComplete(
      (tx: TwitchBitsTransaction) => {
        transactionCallbacks.forEach((cb) => cb(tx));
      }
    );
  }

  return {
    ...toRefs(state),
  };
}

export function useBits(sku: string): void {
  const target = window.Twitch?.ext?.bits;
  if (target) {
    target.useBits(sku);
  }
}
