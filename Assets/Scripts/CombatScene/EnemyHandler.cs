using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyHandler : MonoBehaviour {
    private Dictionary<int, CombatWaveConfig> _waveConfigDictionary;
    private KdTree<EntityBase> _activeEnemies;
    public KdTree<EntityBase> ActiveEnemies { get { return _activeEnemies; } }
    private Dictionary<string, EntityInfo> _enemyInfoDictionary;
    private EntityBase _enemyPrefab;

    private void Awake() {
        var waveConfigArr = Resources.LoadAll<CombatWaveConfig>("ScriptableObjects/CombatWaveConfig");
        _waveConfigDictionary = waveConfigArr.ToDictionary(x => x.WaveRank);

        _activeEnemies = new KdTree<EntityBase>();

        var enemyInformation = Resources.LoadAll<EntityInfo>("ScriptableObjects/Enemies");
        _enemyInfoDictionary = enemyInformation.ToDictionary(x => x.EntityID);

        _enemyPrefab = Resources.Load<EntityBase>("Prefabs/EnemyPrefab");

    }

    public void Progress(KdTree<EntityBase> allies, Truck truck) {
        foreach (EntityBase enemy in _activeEnemies) {
            ITargetable target = allies.FindClosest(enemy.transform.position)?.GetComponent<Agent>();
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
        Vector2 stageMinSize = CombatSceneHandler.StageMinSize;
        Vector2 stageMaxSize = CombatSceneHandler.StageMaxSize;

        int waveRank = stageConfig.StageInfoArray[waveCount - 1];
        CombatWaveConfig waveConfig = _waveConfigDictionary[waveRank];
        CombatWaveInfo selectedWave = waveConfig.WaveInfoArray[Random.Range(0, waveConfig.WaveInfoArray.Length)];
        
        for (int i = 0; i < selectedWave.enemyIDArray.Length; ++i) {
            float randX = Random.Range(stageMinSize.x, stageMaxSize.x);
            float y = Random.Range(0, 2) > 0 ? stageMaxSize.y + _enemyPrefab.Radius : stageMinSize.y - _enemyPrefab.Radius;
            if (waveCount == 1) {
                y = Random.Range(stageMinSize.y / 4f, stageMaxSize.y / 4f);
            }
            
            EntityInfo selectedInfo = _enemyInfoDictionary[selectedWave.enemyIDArray[i]];
            EntityBase enemy = entitySpanwer.CreateEnemy(selectedInfo);
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
