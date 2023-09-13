using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProjectileSpawner : Singleton<ProjectileSpawner>, ILoadable {
    private static readonly string ProjectilePrefabKey = "Projectiles";
    private Dictionary<string, ObjectPool<ProjectileBase>> _projectileObjectPoolDictionary;
    public static bool IsLoadCompleted { 
        get;
        private set;
    }
    public ProjectileSpawner() {
        _projectileObjectPoolDictionary = new Dictionary<string, ObjectPool<ProjectileBase>>();
        AssetLoader.Instance.LoadAssetsAsync<GameObject>(ProjectilePrefabKey, (handle) => {
            OnPrefabLoadCompleted(handle.Result);
            IsLoadCompleted = true;
        });
    }

    public ProjectileBase GetProjectile(string projectileName) {
        ProjectileBase instance = _projectileObjectPoolDictionary[projectileName].GetObject();
        return instance;
    }

    public void RemoveProjectile(ProjectileBase projectile) {
        string key = projectile.name;
        _projectileObjectPoolDictionary[key].ReturnObject(projectile);
    }

    public void RemoveAllProjectiles() {
        foreach (ObjectPool<ProjectileBase> projectilePool in _projectileObjectPoolDictionary.Values) {
            projectilePool.ReturnAllObjects();
        }
    }

    private void OnPrefabLoadCompleted(IList<GameObject> result) {
        IList<ProjectileBase> projectilePrefabs 
            = result
                .Select(x => x.GetComponent<ProjectileBase>())
                .ToList();

        foreach (ProjectileBase projectilePrefab in projectilePrefabs) {
            ObjectPool<ProjectileBase> projectileObjectPool = new ObjectPool<ProjectileBase>(
                200,
                () => { 
                    ProjectileBase instance = GameObject.Instantiate(projectilePrefab);
                    instance.name = projectilePrefab.name;
                    return instance;
                },
                (ProjectileBase x) => x.gameObject.SetActive(true),
                (ProjectileBase x) => x.gameObject.SetActive(false)
            );
            _projectileObjectPoolDictionary.Add(projectilePrefab.name, projectileObjectPool);
        }
    }
}
