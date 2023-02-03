// This module maps globals injected by Webpack (or from other sources)
// to an accessible module usable by our code.

/**
 * @typedef {Object} Globals
 *
 * @property {boolean} ANALYTICS - Boolean if analytics is enabled or not
 * @property {string}  CLIENT_ID - Client ID, as obtained from twitch.
 * @property {boolean} PRODUCTION - true if this is being built in production.
 * @property {string}  UA_STRING - UA string, as obtained from google analytics
 * @property {string | undefined} TESTING_CHANNEL_ID - string
 * @property {string | undefined} TESTING_USER_ID - Boolean if analytics is enabled or not
 * @property {string | undefined} TESTING_PURCHASABLE_ITEMS - JSON string of "purchasable" test items.
 **/

/**
 * @type Globals
 **/
const g = {
  ANALYTICS: import.meta.env.VITE_ANALYTICS,
  CLIENT_ID: import.meta.env.VITE_CLIENT_ID,
  PRODUCTION: import.meta.env.NODE_ENV === "production",
  UA_STRING: import.meta.env.VITE_UA_STRING,

  TESTING_CHANNEL_ID: import.meta.env.VITE_TESTING_CHANNEL_ID,
  TESTING_USER_ID: import.meta.env.VITE_TESTING_USER_ID,
  TESTING_PURCHASABLE_ITEMS: import.meta.env.VITE_TESTING_PURCHASABLE_ITEMS,
};

export default g;
