using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomEvent {
    public class GainGold : IRandomEvent {

        public IEnumerator Execute(string[] variables) {
            int parameterCount = variables.Length;
            if (parameterCount == 1) {
                int amount = int.Parse(variables[0]);
                Gain(amount);
            }
            else if (parameterCount == 2) {
                int min = int.Parse(variables[0]);
                int max = int.Parse(variables[1]);
                Gain(min, max);
            }

            yield break;
        }
        
        private void Gain(int amount) {
            GameMain.PlayerData.AddGold(amount);
        }

        private void Gain(int min, int max) {
            int amount = Random.Range(min, max + 1);
            Gain(amount);
        }
    }

    public class ReduceGold : IRandomEvent {
        public IEnumerator Execute(string[] variables) {
            int parameterCount = variables.Length;
            if (parameterCount == 1) {
                int amount = int.Parse(variables[0]);
                Reduce(amount);
            } 

            yield break;
        }

        private void Reduce(int amount) {
            GameMain.PlayerData.AddGold(-amount);
        }
    }
}