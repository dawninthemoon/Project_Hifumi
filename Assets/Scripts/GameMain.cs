using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {
    private PlayerData _playerData;
    private void Awake() {
        _playerData = new PlayerData();
    }
}
