using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EntitySpawner : ILoadable {
    private ObjectPool<EntityBase> _allyObjectPool;
    private ObjectPool<EntityBase> _enemyObjectPool;
    private CombatDamageDisplay _damageDisplayer;
    
    private static readonly string AllyPrefabName = "AllyPrefab";
    private static readonly string EnemyPrefabName = "EnemyPrefab";
    public static bool IsLoadCompleted { 
        get;
        private set;
    }

    public EntitySpawner(Transform entityParent, CombatDamageDisplay damageDisplayer) {
        _damageDisplayer = damageDisplayer;

        AssetLoader.Instance.LoadAssetAsync<GameObject>(
            AllyPrefabName,
            (op) => OnPrefabLoadCompleted(ref _allyObjectPool, op.Result.GetComponent<EntityBase>(), entityParent)
        );
        AssetLoader.Instance.LoadAssetAsync<GameObject>(
            EnemyPrefabName,
            (op) => OnPrefabLoadCompleted(ref _enemyObjectPool, op.Result.GetComponent<EntityBase>(), entityParent)
        );
    }

    private void OnPrefabLoadCompleted(ref ObjectPool<EntityBase> objectPool, EntityBase prefab, Transform entityParent) {
        objectPool = new ObjectPool<EntityBase>(
            10,
            () => CreateEntityBase(prefab, entityParent),
            OnEntityActive,
            OnEntityDisable
        );
        IsLoadCompleted = true;
    }

    public EntityBase CreateAlly(EntityDecorator entity) {
        EntityBase instance = _allyObjectPool.GetObject();
        instance.Initialize(entity);
        return instance;
    }

    public void RemoveAlly(EntityBase entity) {
        _allyObjectPool.ReturnObject(entity);
    }

    public EntityBase CreateEnemy(EntityDecorator entityDecorator) {
        EntityBase instance = _enemyObjectPool.GetObject();
        instance.Initialize(entityDecorator);
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
        entity.FinalDamageInfo?.Attach(_damageDisplayer);
        //GameMain.PlayerData.Attach(entity);
    }

    private void OnEntityDisable(EntityBase entity) {
        entity.gameObject.SetActive(false);
        entity.FinalDamageInfo?.Detach(_damageDisplayer);
        entity.SetTarget(null);
        //GameMain.PlayerData.Detach(entity);
    }
}
