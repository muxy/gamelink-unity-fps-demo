<template>
  <div class="bits-interface">
    <transition name="fade">
      <div v-if="productList.length > 0">
        <div class="instructions">Click below to start an event!</div>
        <template v-for="product in productList" :key="product.SKU">
          <button @click="confirmSpendWithTwitch(product)">
            Begin {{ product.displayName }} Event for
            {{ product.cost.amount }} Bits!
          </button>
        </template>
      </div>

      <div v-else>No products available.</div>
    </transition>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from "vue";

import analytics from "@/shared/analytics";

import { TwitchBitsProduct } from "@muxy/extensions-js/dist/types/src/twitch";

import { getProducts, useBits } from "@/shared/hooks/use-twitchcontext";

export default defineComponent({
  setup() {
    const productList = ref<TwitchBitsProduct[]>([]);

    const confirmSpendWithTwitch = (product: TwitchBitsProduct) => {
      analytics.sendEvent({
        action: `starting-bits-transaction-SKU:${product.sku}`,
        label: `Starting Bits Transacton: ${product.displayName}`,
        value: product.cost.amount,
      });

      useBits(product.sku);
    };

    onMounted(async () => {
      productList.value = await getProducts();
    });

    return {
      confirmSpendWithTwitch,
      productList,
    };
  },
});
</script>

<style lang="scss">
@import "~@/shared/scss/base.scss";

.bits-interface {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;

  div {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;

    .instructions {
      text-align: center;
      margin-bottom: 16px;
    }
  }
}
</style>
