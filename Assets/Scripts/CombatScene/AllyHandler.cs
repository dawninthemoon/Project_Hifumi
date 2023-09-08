using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class AllyHandler : MonoBehaviour {
    private KdTree<EntityBase> _activeAllies;
    private List<EntityBase> _inactiveAllies;
    private Truck _truck;

    public KdTree<EntityBase> ActiveAllies {
        get { return _activeAllies; }
    }
    public List<EntityBase> InactiveAllies {
        get { return _inactiveAllies; }
    }
    public bool HasActiveAlly {
        get { return _activeAllies.Count > 0; }
    }

    private void Awake() {
        _activeAllies = new KdTree<EntityBase>(true);
        _inactiveAllies = new List<EntityBase>();
    }

    public void SetTruckObject(Truck truck) {
        _truck = truck;
    }

    public void Progress(EnemyHandler enemyHandler) {
        foreach (EntityBase ally in _activeAllies) {
            var activeEnemies = enemyHandler.ActiveEnemies;
            ITargetable target = activeEnemies.FindClosest(ally.transform.position)?.GetComponent<Agent>();
            ally.SetTarget(target);

            ally.transform.position = CombatMap.ClampPosition(ally.transform.position, ally.Radius);
        }

        for (int i = 0; i < _activeAllies.Count; ++i) {
            var ally = _activeAllies[i];
            if (ally.Health <= 0 || !ally.gameObject.activeSelf) {
                _activeAllies[i].SetTarget(null);
                _activeAllies.RemoveAt(i--);
            }
        }
    }

    public void InitalizeAllies(EntitySpawner spawner) {
        foreach (EntityInfo info in GameMain.PlayerData.Allies) {
            EntityBase newEntity = spawner.CreateAlly(info);
            newEntity.gameObject.SetActive(false);
            _inactiveAllies.Add(newEntity);
        }
    }

    public void ActiveAllAllies() {
        for (int i = 0; i < _inactiveAllies.Count; ++i) {
            OnEntityActive(_inactiveAllies[i]);
            i--;
        }

        SoundManager.Instance.PlayBGM("BGM1");
    }

    public void OnEntityActive(EntityBase entity) {
        entity.gameObject.SetActive(true);
        _inactiveAllies.Remove(entity);
        StartCoroutine(StartEaseParabola(entity.transform, () => { _activeAllies.Add(entity); entity.IsUnloadCompleted = true; }));
    }

    public void OnEntityInactive(EntityBase entity) {
        _inactiveAllies.Add(entity);
        entity.SetTarget(null);
        entity.gameObject.SetActive(false);
    }

    public void DisarmAllAllies() {
        for (int i = 0; i < _activeAllies.Count; ++i) {
            _activeAllies[i].SetTarget(null);
        }
    }

    public void RemoveAllAllies(EntitySpawner spawner) {
        for (int i = 0; i < _activeAllies.Count; ++i) {
            spawner.RemoveAlly(_activeAllies[i]);
            _activeAllies.RemoveAt(i--);
        }
        for (int i = 0; i < _inactiveAllies.Count; ++i) {
            spawner.RemoveAlly(_inactiveAllies[i]);
            _inactiveAllies.RemoveAt(i--);
        }
    }

    private IEnumerator StartEaseParabola(Transform target, System.Action callback) {
        float timeAgo = 0f;
        float targetTime = 1f;

        Vector2 start = _truck.transform.position;
        Vector2 end = start;
        end.y -= _truck.Height * 0.75f;
        float xOffset = Random.Range(_truck.Width * 0.25f, _truck.Width);
        xOffset *= Random.Range(0, 2) == 0 ? -1f : 1f;
        end.x += xOffset;

        Vector2 p1 = _truck.Position;
        p1.y += 100f;

        while (timeAgo < targetTime) {
            timeAgo += Time.deltaTime;

            Vector2 p = Bezier.GetPoint(start, p1, end, timeAgo / targetTime);
            target.position = p;

            yield return null;
        }

        callback();
    }
}
