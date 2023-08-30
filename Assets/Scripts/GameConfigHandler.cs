using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigHandler : MonoBehaviour {
    public static float _gameSpeed = 1f;
    public static float GameSpeed { 
        get {
            return _gameSpeed;
        }
        set {
            _gameSpeed = value;
            Time.timeScale = _gameSpeed;
        }
    }
    private void Awake() {
        SoundManager.Instance.EffectVolume = 0.5f;
        SoundManager.Instance.BGMVolume = 0.5f;

        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Entity, true);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.UI, true);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Reward, true);
    }
}
