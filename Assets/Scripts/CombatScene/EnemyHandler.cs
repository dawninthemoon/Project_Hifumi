using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public class EnemyHandler : MonoBehaviour, ILoadable {
    private Dictionary<int, CombatWaveConfig> _waveConfigDictionary;
    private KdTree<EntityBase> _activeEnemies;
    public KdTree<EntityBase> ActiveEnemies { get { return _activeEnemies; } }
    private Dictionary<string, EntityInfo> _enemyInfoDictionary;
    private EntityBase _enemyPrefab;
    public static bool IsLoadCompleted {
        get;
        private set;
    } = false;

    private async UniTaskVoid Awake() {
        _activeEnemies = new KdTree<EntityBase>();

        var assetLoader = AssetLoader.Instance;

        IList<CombatWaveConfig> waveConfigList 
            = await assetLoader.LoadAssetsAsync<CombatWaveConfig>("CombatConfig");
        _waveConfigDictionary = waveConfigList.ToDictionary(x => x.WaveRank);

        IList<EntityInfo> enemyInformation 
            = await assetLoader.LoadAssetsAsync<EntityInfo>("EnemiesConfig");
        _enemyInfoDictionary = enemyInformation.ToDictionary(x => x.EntityID);

        _enemyPrefab 
            = (await assetLoader.LoadAssetAsync<GameObject>("EnemyPrefab"))
                .GetComponent<EntityBase>();

        IsLoadCompleted = true;
    }

    public void Progress(KdTree<EntityBase> allies, Truck truck) {
        foreach (EntityBase enemy in _activeEnemies) {
            EntityBase targetEntity = allies.FindClosest(enemy.transform.position);
            if (targetEntity != null && !targetEntity.IsUnloadCompleted) {
                continue;
            }
            ITargetable target = targetEntity?.GetComponent<Agent>();
            if (target == null && truck.MoveProgressEnd) {
                target = truck;
            }

            enemy.SetTarget(target);
        }

        for (int i = 0; i < _activeEnemies.Count; ++i) {
            var enemy = _activeEnemies[i];
            if (enemy.Health <= 0 || !enemy.gameObject.activeSelf) {
                _activeEnemies.RemoveAt(i--);
            }
        }
    }

    public void SpawnEnemies(int waveCount, CombatStageConfig stageConfig, EntitySpawner entitySpanwer) {
        Vector2 stageMinSize = CombatMap.StageMinSize;
        Vector2 stageMaxSize = CombatMap.StageMaxSize;

        int waveRank = stageConfig.StageInfoArray[waveCount - 1];
        CombatWaveConfig waveConfig = _waveConfigDictionary[waveRank];
        CombatWaveInfo selectedWave = waveConfig.WaveInfoArray[Random.Range(0, waveConfig.WaveInfoArray.Length)];
        
        for (int i = 0; i < selectedWave.enemyIDArray.Length; ++i) {
            float randX = Random.Range(stageMinSize.x, stageMaxSize.x);
            float y;
            if (waveCount == 1) {
                y = Random.Range(stageMinSize.y / 4f, stageMaxSize.y / 4f);
            }
            else {
                 y = Random.Range(0, 2) > 0 ? stageMaxSize.y + _enemyPrefab.Radius : stageMinSize.y - _enemyPrefab.Radius;
            }
            
            EntityInfo selectedInfo = _enemyInfoDictionary[selectedWave.enemyIDArray[i]];
            EntityDecorator decorator = new EntityDecorator(selectedInfo);
            EntityBase enemy = entitySpanwer.CreateEnemy(decorator);
            enemy.transform.position = new Vector3(randX, y);

            _activeEnemies.Add(enemy);
        }
    }

    public void RemoveAllEnemies(EntitySpawner entitySpanwer) {
        for (int i = 0; i < _activeEnemies.Count; ++i) {
            entitySpanwer.RemoveEnemy(_activeEnemies[i]);
            _activeEnemies.RemoveAt(i--);
        }
    }
}
