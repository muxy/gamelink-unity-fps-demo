import { inject, provide, InjectionKey } from "vue";

import Muxy, { MuxySDK, SetupOptions } from "@muxy/extensions-js";
import { DebuggingOptions } from "@muxy/extensions-js/dist/types/src/debug";

// Muxy Debug extension hook
declare global {
  interface Window {
    __VUE_MUXY_DEV_HOOK: (
      muxy: typeof Muxy,
      debug: DebuggingOptions,
      setup: SetupOptions
    ) => Record<string, unknown>;
  }
}

// MEDKit Options
interface VueMEDKitOptions {
  clientId: string;

  channelId?: string;
  environment?: string;
  jwt?: string;
  role?: string;
  uaString?: string;
  url?: string;
  userId?: string;
}

const MEDKitInjectionKey: InjectionKey<MuxySDK> = Symbol("medkit");

export function provideMEDKit(options: VueMEDKitOptions): MuxySDK {
  if (!options.clientId) {
    throw new Error("Must specify client id when using the MEDKit Vue plugin");
  }

  const opts = new Muxy.DebuggingOptions();
  opts.role(options.role || "viewer");

  if (options.environment) {
    opts.environment(options.environment);
  }

  if (options.jwt) {
    opts.jwt(options.jwt);
  }

  if (options.channelId) {
    opts.channelID(options.channelId);
  }

  if (options.userId) {
    opts.userID(options.userId);
  }

  if (options.url) {
    opts.url(options.url);
  }

  const setup: SetupOptions = {
    clientID: options.clientId,
  };

  if (options.uaString) {
    setup.uaString = options.uaString;
  }

  if (window.__VUE_MUXY_DEV_HOOK) {
    window.__VUE_MUXY_DEV_HOOK(Muxy, opts, setup);
  } else {
    Muxy.debug(opts);
    Muxy.setup(setup);
  }

  const medkit = new Muxy.SDK();
  provide<MuxySDK>(MEDKitInjectionKey, medkit);

  return medkit;
}

export function useMEDKit(): { medkit: MuxySDK } {
  const medkit = inject<MuxySDK>(MEDKitInjectionKey);
  if (!medkit) {
    throw new Error("MEDKit could not be created");
  }

  return { medkit };
}
