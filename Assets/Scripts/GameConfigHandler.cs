using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigHandler : MonoBehaviour {
    private void Awake() {
        SoundManager.Instance.EffectVolume = 0.5f;
        SoundManager.Instance.BGMVolume = 0.5f;
    }
}
