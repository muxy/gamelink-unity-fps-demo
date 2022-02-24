/* eslint-disable */

const path = require("path");
const ZipPlugin = require("zip-webpack-plugin");

const pkg = require("./package.json");

const config = {
  publicPath: "./",
  pages: {
    config: "./src/config/main.js",
    overlay: "./src/overlay/main.js",
  },

  chainWebpack: config => {
    config.resolve.alias.set("@", path.resolve(__dirname, "src"));

    // Zip Plugin
    config
      .plugin("zip-plugin")
      .use(ZipPlugin, [{ filename: `${pkg.name}.zip` }]);

    // ESLint autofix
    config.module
      .rule("eslint")
      .use("eslint-loader")
      .options({
        fix: true,
      });
  },

  devServer: {
    host: "0.0.0.0",
    port: 4000,
  },
};

module.exports = config;
