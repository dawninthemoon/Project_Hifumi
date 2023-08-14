using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class GameTest : MonoBehaviour {
    [SerializeField] private int _entityCount = 2;
    [SerializeField] private EntityBase _enemyPrefabMelee = null, _enemyPrefabRange = null;
    [SerializeField] private CustomCollider[] _obstacleArea = null;
    private KdTree<EntityBase> _allies;
    private KdTree<EntityBase> _enemies;
    private List<EntityBase> _allEntityBases;
    public static readonly float Width = 640;
    public static readonly float Height = 380f;
    private static Vector2 _stageMinSize;
    private static Vector2 _stageMaxSize;
    public static Vector2 StageMinSize { get { return _stageMinSize; } }
    public static Vector2 StageMaxSize { get { return _stageMaxSize; } }
    private float _gameSpeed;

    private void Awake() {
        var memberUITest = GameObject.FindObjectOfType<MemberUITest>();
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

            EntityBase enemy = null;

            while (true) {
                Vector3 randPos = new Vector3(Random.Range(_stageMinSize.x, _stageMaxSize.x), Random.Range(_stageMinSize.y, _stageMaxSize.y));

                EntityBase enemyPrefab = _enemyPrefabMelee;
                if (Random.Range(0, 2) > 0) {
                    enemyPrefab = _enemyPrefabRange;
                }
                if (!CanCreateEntity(randPos, enemyPrefab)) continue;

                enemy = Instantiate(enemyPrefab, randPos, Quaternion.identity);

                break;
            }

            _enemies.Add(enemy);
            _allEntityBases.Add(enemy);
        }
    }

    private void Update() {
        MoveProgress();

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

    private void LateUpdate() {
        for (int i = 0; i < _allEntityBases.Count; ++i) {
            var entity = _allEntityBases[i];
            if (entity.Health <= 0 || !entity.gameObject.activeSelf) {
                _allEntityBases.RemoveAt(i--);
            }
        }

        for (int i = 0; i < _enemies.Count; ++i) {
            var enemy = _enemies[i];
            if (enemy.Health <= 0 || !enemy.gameObject.activeSelf) {
                _enemies.RemoveAt(i--);
            }
        }

        for (int i = 0; i < _allies.Count; ++i) {
            var ally = _allies[i];
            if (ally.Health <= 0 || !ally.gameObject.activeSelf) {
                _allies.RemoveAt(i--);
            }
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

    public void OnEntityCreated(EntityBase entity) {
        _allies.Add(entity);
        _allEntityBases.Add(entity);
    }

    public bool CanCreateEntity(Vector3 position, EntityBase entity) {
        if (_obstacleArea == null) return true;
/*
        Vector3 prevPosition = entity.transform.position;
        entity.transform.position = position;
        foreach (CustomCollider obstacle in _obstacleArea) {
            if (entity.IsCollision(obstacle)) {
                entity.transform.position = prevPosition;
                return false;
            }
        }
        entity.transform.position = prevPosition;*/

        return true;
    }
}
