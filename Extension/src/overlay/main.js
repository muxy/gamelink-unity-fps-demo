import { createApp } from "vue";

import App from "./App.vue";

window.MEDKIT_PURCHASABLE_ITEMS = [
  {
    sku: "spawn-hoverbot",
    displayName: "Spawn Extra HP Hoverbot",
    cost: {
      amount: 50,
      type: "test-cost",
    },
  },
  {
    sku: "spawn-turret",
    displayName: "Spawn Extra HP Turret",
    cost: {
      amount: 100,
      type: "test-cost",
    },
  },
];

createApp(App).mount("#app");
