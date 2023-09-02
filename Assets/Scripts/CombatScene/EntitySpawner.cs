using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner {
    private ObjectPool<EntityBase> _allyObjectPool;
    private ObjectPool<EntityBase> _enemyObjectPool;
    
    private static readonly string AllyPrefabPath = "Prefabs/AllyPrefab";
    private static readonly string EnemyPrefabPath = "Prefabs/EnemyPrefab";

    public EntitySpawner(Transform entityParent) {
        EntityBase allyPrefab = Resources.Load<EntityBase>(AllyPrefabPath);
        EntityBase enemyPrefab = Resources.Load<EntityBase>(EnemyPrefabPath);

        _allyObjectPool = new ObjectPool<EntityBase>(
            10,
            () => CreateEntityBase(allyPrefab, entityParent),
            OnEntityActive,
            OnEntityDisable
        );
        _enemyObjectPool = new ObjectPool<EntityBase>(
            10,
            () => CreateEntityBase(enemyPrefab, entityParent),
            OnEntityActive,
            OnEntityDisable
        );
    }

    public EntityBase CreateAlly(EntityInfo entityInfo) {
        EntityBase instance = _allyObjectPool.GetObject();
        instance.Initialize(entityInfo);
        return instance;
    }

    public void RemoveAlly(EntityBase entity) {
        _allyObjectPool.ReturnObject(entity);
    }

    public EntityBase CreateEnemy(EntityInfo entityInfo) {
        EntityBase instance = _enemyObjectPool.GetObject();
        instance.Initialize(entityInfo);
        return instance;
    }

    public void RemoveEnemy(EntityBase entity) {
        _enemyObjectPool.ReturnObject(entity);
    }

    private EntityBase CreateEntityBase(EntityBase prefab, Transform entityParent) {
        EntityBase instance = GameObject.Instantiate(prefab, entityParent);
        return instance;
    }

    private void OnEntityActive(EntityBase entity) {
        entity.gameObject.SetActive(true);
        GameMain.PlayerData.Attach(entity);
    }

    private void OnEntityDisable(EntityBase entity) {
        entity.gameObject.SetActive(false);
        GameMain.PlayerData.Detach(entity);
    }
}
