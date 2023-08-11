using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class GameTest : MonoBehaviour {
    [SerializeField] private int _entityCount = 2;
    [SerializeField] private EntityBase _allyPrefabMelee = null, _allyPrefabRange = null;
    [SerializeField] private EntityBase _enemyPrefabMelee = null, _enemyPrefabRange = null;
    private KdTree<EntityBase> _allies;
    private KdTree<EntityBase> _enemies;
    private List<EntityBase> _allEntityBases;
    public static readonly float Width = 18f;
    public static readonly float Height = 10f;
    private static Vector2 _stageMinSize;
    private static Vector2 _stageMaxSize;
    public static Vector2 StageMinSize { get { return _stageMinSize; } }
    public static Vector2 StageMaxSize { get { return _stageMaxSize; } }
    private float _gameSpeed;

    private void Awake() {
    }

    private void Start() {
        InitializeCombat();
    }

    private void InitializeCombat() {
        if (_allEntityBases != null) {
            foreach (var toRemove in _allEntityBases) {
                Destroy(toRemove.gameObject);
            }
        }

        _allies = new KdTree<EntityBase>(true);
        _enemies = new KdTree<EntityBase>(true);
        _allEntityBases = new List<EntityBase>();

        for (int i = 0; i < _entityCount; ++i) {
            _stageMinSize = new Vector2(-Width / 2f, -Height / 2f);
            _stageMaxSize = new Vector2(Width / 2f, Height / 2f);

            Vector3 randPos1 = new Vector3(Random.Range(_stageMinSize.x, _stageMaxSize.x), Random.Range(_stageMinSize.y, _stageMaxSize.y));
            Vector3 randPos2 = new Vector3(Random.Range(_stageMinSize.x, _stageMaxSize.x), Random.Range(_stageMinSize.y, _stageMaxSize.y));

            EntityBase allyPrefab = _allyPrefabMelee;
            if (Random.Range(0, 2) > 0) {
                allyPrefab = _allyPrefabRange;
            }
            EntityBase enemyPrefab = _enemyPrefabMelee;
            if (Random.Range(0, 2) > 0) {
                enemyPrefab = _enemyPrefabRange;
            }

            var ally = Instantiate(allyPrefab, randPos1, Quaternion.identity);
            var enemy = Instantiate(enemyPrefab, randPos2, Quaternion.identity);

            _allies.Add(ally);
            _enemies.Add(enemy);
            _allEntityBases.Add(ally);
            _allEntityBases.Add(enemy);
        }
    }

    private void Update() {
        MoveProgress();

        for (int i = 0; i < _allEntityBases.Count; ++i) {
            var EntityBase = _allEntityBases[i];
            if (EntityBase.Health <= 0) {
                _allEntityBases.RemoveAt(i--);
                EntityBase.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _enemies.Count; ++i) {
            var enemy = _enemies[i];
            if (!enemy.gameObject.activeSelf) {
                _enemies.RemoveAt(i--);
                Destroy(enemy.gameObject);
            }
        }

        for (int i = 0; i < _allies.Count; ++i) {
            var ally = _allies[i];
            if (!ally.gameObject.activeSelf) {
                _allies.RemoveAt(i--);
                Destroy(ally.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            InitializeCombat();
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            _gameSpeed = Mathf.Max(0.5f, _gameSpeed - 0.5f);
            Time.timeScale = _gameSpeed;
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            _gameSpeed = Mathf.Min(10f, _gameSpeed + 0.5f);
            Time.timeScale = _gameSpeed;
        }
    }

    private void MoveProgress() {
        foreach (EntityBase ally in _allies) {
            EntityBase target = _enemies.FindClosest(ally.transform.position);
            if (ally.DoingAttack || target == null) {
                ally.SetMoveAnimationState(false);
                continue;
            }

            ally.Move(target);

            ClampPosition(ally);
        }

        foreach (EntityBase enemy in _enemies) {
            EntityBase target = _allies.FindClosest(enemy.transform.position);
            if (enemy.DoingAttack || target == null) {
                enemy.SetMoveAnimationState(false);
                continue;
            }

            enemy.Move(target);
            
            ClampPosition(enemy);
        }
    }

    private void ClampPosition(EntityBase entity) {
        Vector2 pos = entity.transform.position;
        pos.x = Mathf.Clamp(pos.x, _stageMinSize.x + entity.Radius, _stageMaxSize.x - entity.Radius);
        pos.y = Mathf.Clamp(pos.y, _stageMinSize.y + entity.Radius, _stageMaxSize.y - entity.Radius);
        entity.transform.position = pos;
    }
}
