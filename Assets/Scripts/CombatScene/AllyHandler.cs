using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class AllyHandler : MonoBehaviour {
    private KdTree<EntityBase> _activeAllies;
    private List<EntityBase> _inactiveAllies;
    private Truck _truck;
    private SynergyHandler _synergyHandler;

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
        _synergyHandler = new SynergyHandler();
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
        foreach (EntityDecorator entity in GameMain.PlayerData.Member) {
            EntityBase newEntity = spawner.CreateAlly(entity);
            newEntity.gameObject.SetActive(false);
            _inactiveAllies.Add(newEntity);
        }
    }

    public void ActiveAllAllies() {
        for (int i = 0; i < _inactiveAllies.Count; ++i) {
            OnEntityActive(_inactiveAllies[i]);
            i--;
        }
        ApplySynergies();

        SoundManager.Instance.PlayBGM("BGM1");
    }

    public void OnEntityActive(EntityBase entity) {
        entity.gameObject.SetActive(true);
        _inactiveAllies.Remove(entity);
        _activeAllies.Add(entity); 
        // 넣었다 뺄 때마다 시너지가 바뀔 것을 상정. 나중에 수정 필요함.
        _synergyHandler.AddSynergy(entity, true);
        
        StartCoroutine(
            StartEaseParabola(
                entity.transform,
                () => {
                    entity.IsUnloadCompleted = true; 
                }
            )
        );
    }

    public void OnEntityInactive(EntityBase entity) {
        _inactiveAllies.Add(entity);
        entity.SetTarget(null);
        _synergyHandler.AddSynergy(entity, false);
        ApplySynergies();

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

    public void ApplySynergies() {
        int starts = (int)SynergyType.None + 1;
        int ends = (int)SynergyType.Count;
        for (int i = starts; i < ends; ++i) {
            SynergyType type = (SynergyType)i;
            (BuffConfig, BuffConfig) buffPair = _synergyHandler.GetSynergyBuffPair(type);
            if (buffPair.Item2 != null) {
                ApplyBuffToAll(buffPair.Item2, buffPair.Item1);
            }
        }
    }

    private void ApplyBuffToAll(BuffConfig toApply, BuffConfig toRemove) {
        foreach (EntityBase entity in _activeAllies) {
            entity.BuffControl.AddBuff(toApply);
            if (toRemove != null) {
                entity.BuffControl.RemoveBuff(toRemove);
            }
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
