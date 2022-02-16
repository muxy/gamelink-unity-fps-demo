export function isDebug(): boolean {
  // eslint-disable-next-line
    return !!window.__VUE_MUXY_DEV_HOOK;
}

export function isDebugUser(id: string | undefined): boolean {
  return true;
  /*
  if (id) {
    return [
      "12345678", // Fake User
      "126955211", // Muxy
      "89319907", // Muxy01
      "70593331", // capt_smythe
      "119631962", // karlaplan
      "73972968", // camr
      "26052853", // rrrrawwr
      "224146883", // MattSac0
    ].includes(id);
  }

  return false;
  */
}
