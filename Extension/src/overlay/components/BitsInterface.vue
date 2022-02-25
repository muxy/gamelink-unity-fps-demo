<template>
  <div class="bits-interface">
    <transition name="fade">
      <div v-if="productList.length > 0">
        <template v-for="product in productList" :key="product.SKU">
          <button @click="confirmSpendWithTwitch(product)">
            {{ product.displayName }}:
            <strong>{{ product.cost.amount }}</strong> Bits!
          </button>
        </template>
      </div>
    </transition>
  </div>
</template>

<script lang="js">
import { defineComponent, ref } from "vue";

import { useMEDKit } from "@/shared/hooks/use-medkit";

export default defineComponent({
  setup() {
    const { medkit } = useMEDKit();
    const productList = ref([]);

    const confirmSpendWithTwitch = (product) => {
      medkit.purchase(product.sku);
    };

    medkit.getProducts().then((products) => {
      productList.value = products;
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

  width: 100%;

  div {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;

    width: 100%;

    .instructions {
      text-align: center;
      margin-bottom: 16px;
    }

    button {
      width: 100%;
    }
  }
}
</style>
