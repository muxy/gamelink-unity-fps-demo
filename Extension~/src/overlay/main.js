import { createApp } from "vue";

import App from "./App.vue";

window.MEDKIT_PURCHASABLE_ITEMS = [
  {
    sku: "test-sku-10",
    displayName: "Spawn Hoverbot with Bits",
    cost: {
      amount: 50,
      type: "test-cost"
    }
  },
  {
    sku: "test-sku-20",
    displayName: "Spawn Turret with Bits",
    cost: {
      amount: 100,
      type: "test-cost"
    }
  }
];

createApp(App).mount("#app");
