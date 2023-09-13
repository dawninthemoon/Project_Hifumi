using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

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

    private static readonly string AllyDataKey = "AlliesConfig";
    private static readonly string ItemDataKey = "EntityItems";

    private async UniTaskVoid Awake() {
        _playerData = GetComponent<PlayerData>();

        var assetLoader = AssetLoader.Instance;
        var alliesData = await assetLoader.LoadAssetsAsync<EntityInfo>(AllyDataKey);
        var itemsData = await assetLoader.LoadAssetsAsync<Belongings>(ItemDataKey);

        _playerData.Allies.Add(alliesData[0]);

        RewardData = new RewardData(alliesData, itemsData);

        var spawner = ProjectileSpawner.Instance;
        var display = CombatDamageDisplay.Instance;
        var sound = SoundManager.Instance;
    }

    private void Start() {
        if (!_isTestCode)
            _gameMap.StartGenerateMap();
    }
}
