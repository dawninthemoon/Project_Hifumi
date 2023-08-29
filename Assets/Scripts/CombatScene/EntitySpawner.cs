using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner {
    private ObjectPool<EntityBase> _allyObjectPool;
    private ObjectPool<EntityBase> _enemyObjectPool;
    
    private static readonly string AllyPrefabPath = "Prefabs/AllyPrefab";
    private static readonly string EnemyPrefabPath = "Prefabs/EnemyPrefab";

    public EntitySpawner() {
        EntityBase allyPrefab = Resources.Load<EntityBase>(AllyPrefabPath);
        EntityBase enemyPrefab = Resources.Load<EntityBase>(EnemyPrefabPath);

        _allyObjectPool = new ObjectPool<EntityBase>(
            10,
            () => CreateEntityBase(allyPrefab),
            OnEntityActive,
            OnEntityDisable
        );
        _enemyObjectPool = new ObjectPool<EntityBase>(
            10,
            () => CreateEntityBase(enemyPrefab),
            OnEntityActive,
            OnEntityDisable
        );
    }

    public EntityBase CreateAlly(EntityInfo entityInfo) {
        EntityBase instance = _allyObjectPool.GetObject();
        instance.Initialize(entityInfo);
        return instance;
    }

    public EntityBase CreateEnemy(EntityInfo entityInfo) {
        EntityBase instance = _enemyObjectPool.GetObject();
        instance.Initialize(entityInfo);
        return instance;
    }

    private EntityBase CreateEntityBase(EntityBase prefab) {
        EntityBase instance = GameObject.Instantiate(prefab);
        return instance;
    }

    private void OnEntityActive(EntityBase entity) {
        entity.gameObject.SetActive(true);
    }

    private void OnEntityDisable(EntityBase entity) {
        entity.gameObject.SetActive(false);
    }
}