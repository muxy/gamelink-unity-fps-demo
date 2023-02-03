import { inject, provide, reactive } from "vue";

const StateInjectionKey = Symbol("state");

export function provideState(medkit) {
  const state = reactive(channelStateFromNetwork());

  provide(StateInjectionKey, state);

  return new Promise((resolve) => {
    medkit.loaded().then(() => {
      medkit.getChannelState().then((st) => {
        try {
          Object.assign(state, channelStateFromNetwork(st));
        } catch (err) {
          console.error(err);
        }

        // Certain events will be broadcast automatically in response to
        // server actions. This event is sent whenever the channel state
        // is changed by the broadcaster.
        medkit.listen("channel_state_update", (st) => {
          try {
            Object.assign(state, channelStateFromNetwork(st));
          } catch (err) {
            console.error(err);
          }
        });

        resolve(state);
      });
    });
  });
}

export function useState() {
  const state = inject(StateInjectionKey);
  if (!state) {
    throw new Error("State could not be found");
  }

  return { state };
}

export function channelStateFromNetwork(data) {
  return {
    hoverBotsKilled: data?.hoverBotsKilled || 0,
    turretsKilled: data?.turretsKilled || 0,
  };
}
