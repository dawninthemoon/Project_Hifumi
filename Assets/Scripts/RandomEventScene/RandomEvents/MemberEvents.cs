using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomEvent {
    public class AddRandomMember : IRandomEvent {
        public void Execute(string[] variables) {
            EntityInfo newAlly = GameMain.RewardData.GetRandomAlly(true);
            GameMain.PlayerData.AddMember(newAlly);
        }
    }
    public class LoseRandomMember : IRandomEvent {
        public void Execute(string[] variables) {
            GameMain.PlayerData.RemoveRandomMember();
        }
    }
}
