export interface ChannelState {
  enemiesKilled: number;
  healthpacksPickedUp: number;
}

export interface DatastreamEvent {
  spawnMonsterType: string;
  spawnPickupType: string;
}
// export class ChannelState {
//   public enemiesKilled = ref(0);

//   public update(data: ChannelStateNetwork) {
//     if (data.enemiesKilled) {
//       this.enemiesKilled.value = data.enemiesKilled;
//     }
//   }

//   public clear() {
//     this.enemiesKilled.value = 0;
//   }
// }
