using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : Singleton<ProjectileSpawner> {
    private static readonly string ProjectilePrefabPath = "Prefabs/Projectiles";
    private Dictionary<string, ObjectPool<ProjectileBase>> _projectileObjectPoolDictionary;
    public ProjectileSpawner() {
        _projectileObjectPoolDictionary = new Dictionary<string, ObjectPool<ProjectileBase>>();
        ProjectileBase[] projectilePrefabs = Resources.LoadAll<ProjectileBase>(ProjectilePrefabPath);
        foreach (ProjectileBase projectilePrefab in projectilePrefabs) {
            ObjectPool<ProjectileBase> projectileObjectPool = new ObjectPool<ProjectileBase>(
                30,
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
}
