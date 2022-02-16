import { ref } from "vue";
import { MuxySDK } from "@muxy/extensions-js";

interface AnalyticsEvent {
  action: string;
  value: number;
  label: string;
}

const medkit = ref<MuxySDK | null>(null);
const eventQueue: AnalyticsEvent[] = [];
let eventTimer: number | undefined = undefined;

// Send a keep alive heartbeat to the analytics system every minute.
const HEARTBEAT_TIMEOUT_MS = 60 * 1000;

// If the event queue has an item, send it.
const pumpEventQueue = async () => {
  eventTimer = undefined;

  if (eventQueue.length === 0) {
    return;
  }

  if (!medkit.value) {
    eventTimer = window.setTimeout(pumpEventQueue, 500);
    return;
  }

  eventQueue.shift();

  if (eventQueue.length > 0) {
    eventTimer = window.setTimeout(pumpEventQueue, 500);
  }
};

export default {
  setMedkit(appMedkit: MuxySDK): void {
    medkit.value = appMedkit;
  },

  // Sends a single event namespaced to the provided category.
  async sendEvent(event: AnalyticsEvent): Promise<void> {
    eventQueue.push(event);

    if (!eventTimer) {
      eventTimer = window.setTimeout(pumpEventQueue, 500);
    }
  },

  // Starts a cycle of sending live heartbeat events.
  startKeepAliveHeartbeat(category: string): void {
    this.sendEvent({
      action: "heartbeat",
      value: 1,
      label: "Viewer heartbeat",
    });

    window.setTimeout(() => {
      this.startKeepAliveHeartbeat(category);
    }, HEARTBEAT_TIMEOUT_MS);
  },

  // Send a single "pageview" event.
  async sendPageView(medkit: MuxySDK): Promise<void> {
    if (!medkit.analytics) {
      return Promise.resolve();
    }

    await medkit.loaded();
    return medkit.analytics.pageView();
  },
};
