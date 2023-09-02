using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {
    private static PlayerData _playerData;
    public static PlayerData PlayerData {
        get { return _playerData; }
    }
    [SerializeField] private GameMap _gameMap = null;
    [SerializeField] private EntityInfo[] _defaultEntities = null;
    [SerializeField] private Belongings[] _defaultBelongings = null;
    [SerializeField] private bool _isTestCode = false;

    private void Awake() {
        _playerData = new PlayerData();
        InitializeAllies();
    }

    private void Start() {
        if (!_isTestCode)
            _gameMap.StartGenerateMap();
    }

    private void InitializeAllies() {
        if (_defaultEntities == null || _defaultEntities.Length == 0) {
            var entityInformation = Resources.LoadAll<EntityInfo>("ScriptableObjects/Allies");
            _playerData.Allies.Add(entityInformation[0]);
        }
        else { 
            // For Test
            foreach (EntityInfo info in _defaultEntities) {
                _playerData.Allies.Add(info);
            }
        }
    }

    private void InitializeBelongings() {
        if (_defaultBelongings != null) {
            foreach (Belongings item in _defaultBelongings) {
                _playerData.AddBelongingsInInventory(item);
            }
        }
    }
}
