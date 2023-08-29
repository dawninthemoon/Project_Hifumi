using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {
    private static PlayerData _playerData;
    public static PlayerData PlayerData {
        get { return _playerData; }
    }
    [SerializeField] private GameMap _gameMap = null;
    [SerializeField] private bool _isTestCode = false;

    private void Awake() {
        _playerData = new PlayerData();
        InitializeAllies();
    }

    private void Start() {
        if (!_isTestCode)
            _gameMap.StartGenerateMap();
    }

    // For Test
    private void InitializeAllies() {
        var entityInformation = Resources.LoadAll<EntityInfo>("ScriptableObjects/Allies");
        _playerData.Allies.Add(entityInformation[0]);
    }
}
