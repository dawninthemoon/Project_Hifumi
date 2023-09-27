using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomEvent {
    public class AddRandomMember : IRandomEvent {
        public IEnumerator Execute(string[] variables) {
            EntityInfo newAlly = GameMain.RewardData.GetRandomAlly(true);
            GameMain.PlayerData.AddMember(newAlly);

            yield break;
        }
    }
    public class LoseRandomMember : IRandomEvent {
        public IEnumerator Execute(string[] variables) {
            GameMain.PlayerData.RemoveRandomMember();

            yield break;
        }
    }
}
