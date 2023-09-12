using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {
    private static PlayerData _playerData;
    public static PlayerData PlayerData {
        get { return _playerData; }
    }
    public static RewardData RewardData {
        get;
        private set;
    }
    [SerializeField] private GameMap _gameMap = null;
    [SerializeField] private bool _isTestCode = false;

    private void Awake() {
        _playerData = GetComponent<PlayerData>();
        RewardData = new RewardData(_playerData);
        InitializeAllies();
    }

    private void Start() {
        if (!_isTestCode)
            _gameMap.StartGenerateMap();
    }

    private void InitializeAllies() {
        if (_playerData.Allies.Count == 0) {
            var entityInformation = Resources.LoadAll<EntityInfo>("ScriptableObjects/Allies");
            _playerData.Allies.Add(entityInformation[0]);
        }
    }
}
