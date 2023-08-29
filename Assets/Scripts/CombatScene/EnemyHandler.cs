using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyHandler : MonoBehaviour {
    private Dictionary<int, CombatWaveConfig> _waveConfigDictionary;
    private KdTree<EntityBase> _activeEnemies;
    public KdTree<EntityBase> ActiveEnemies { get { return _activeEnemies; } }

    private void Awake() {
        var waveConfigArr = Resources.LoadAll<CombatWaveConfig>("ScriptableObjects/CombatWaveConfig");
        _waveConfigDictionary = waveConfigArr.ToDictionary(x => x.WaveRank);

        _activeEnemies = new KdTree<EntityBase>();
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
        EntityBase enemyPrefab = Resources.Load<EntityBase>("Prefabs/EnemyPrefab");
        var entityInformation = Resources.LoadAll<EntityInfo>("ScriptableObjects/Enemies");

        Vector2 stageMinSize = CombatSceneHandler.StageMinSize;
        Vector2 stageMaxSize = CombatSceneHandler.StageMaxSize;

        int amount = stageConfig.StageInfoArray.Length;
        for (int i = 0; i < amount; ++i) {
            int randomIndex = Random.Range(0, entityInformation.Length);

            float randX = Random.Range(stageMinSize.x, stageMaxSize.x);
            float y = Random.Range(0, 2) > 0 ? stageMaxSize.y + enemyPrefab.Radius : stageMinSize.y - enemyPrefab.Radius;
            if (waveCount == 1) {
                y = Random.Range(stageMinSize.y / 4f, stageMaxSize.y / 4f);
            }
            
            EntityBase enemy = entitySpanwer.CreateEnemy(entityInformation[randomIndex]);
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
