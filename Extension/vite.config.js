import vue from "@vitejs/plugin-vue";

import { fileURLToPath } from "url";
import { defineConfig } from "vite";

// https://vitejs.dev/config/
export default defineConfig({
  base: "./",

  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },

  server: {
    port: 3000,
  },

  plugins: [vue()],

  build: {
    rollupOptions: {
      input: {
        config: fileURLToPath(new URL("./config.html", import.meta.url)),
        overlay: fileURLToPath(new URL("./overlay.html", import.meta.url)),
      },
    },
  },
});
