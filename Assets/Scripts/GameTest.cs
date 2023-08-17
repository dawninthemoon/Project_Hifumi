using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class GameTest : MonoBehaviour {
    [SerializeField] private MemberUIControl _memberUIControl = null;
    [SerializeField] private int _entityCount = 2;
    [SerializeField] private EntityBase _enemyPrefabMelee = null, _enemyPrefabRange = null;
    private KdTree<EntityBase> _activeAllies;
    private KdTree<EntityBase> _activeEnemies;
    private List<EntityBase> _inactiveAllies;
    public static readonly float Width = 640;
    public static readonly float Height = 380f;
    private static Vector2 _stageMinSize;
    private static Vector2 _stageMaxSize;
    public static Vector2 StageMinSize { get { return _stageMinSize; } }
    public static Vector2 StageMaxSize { get { return _stageMaxSize; } }
    private float _gameSpeed;

    private void Awake() {
        _inactiveAllies = InitalizeEntities();
    }

    private void Start() {
        _memberUIControl.InitializeEntityUI(OnEntityActive, OnEntityInactive, _inactiveAllies);
        InitializeCombat();
    }

    private void InitializeCombat() {
        _activeAllies = new KdTree<EntityBase>(true);
        _activeEnemies = new KdTree<EntityBase>(true);

        _stageMinSize = new Vector2(-Width / 2f, -Height / 2f);
        _stageMaxSize = new Vector2(Width / 2f, Height / 2f);
    
        SpawnEnemy(_entityCount);
    }

    private List<EntityBase> InitalizeEntities() {
        List<EntityBase> inactiveAllies = new List<EntityBase>();
        var entityPrefabs = Resources.LoadAll<EntityBase>("Prefabs/Allies");
        foreach (EntityBase prefab in entityPrefabs) {
            EntityBase newEntity = Instantiate(prefab);
            newEntity.Initialize();
            newEntity.gameObject.SetActive(false);
            inactiveAllies.Add(newEntity);
        }
        return inactiveAllies;
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

    private void MoveProgress() {
        foreach (EntityBase ally in _activeAllies) {
            EntityBase target = _activeEnemies.FindClosest(ally.transform.position);
            if (target == null) break;
            ally.SetTarget(target);

            ClampPosition(ally);
        }

        foreach (EntityBase enemy in _activeEnemies) {
            EntityBase target = _activeAllies.FindClosest(enemy.transform.position);
            if (target == null) break;
            enemy.SetTarget(target);
            
            ClampPosition(enemy);
        }
    }

    private void LateUpdate() {
        foreach (EntityBase inactiveAlly in _inactiveAllies) {
            _memberUIControl.UpdateMemberElement(inactiveAlly);
        }

        for (int i = 0; i < _activeAllies.Count; ++i) {
            var ally = _activeAllies[i];
            if (ally.Health <= 0 || !ally.gameObject.activeSelf) {
                _activeAllies.RemoveAt(i--);
            }
        }

        for (int i = 0; i < _activeEnemies.Count; ++i) {
            var enemy = _activeEnemies[i];
            if (enemy.Health <= 0 || !enemy.gameObject.activeSelf) {
                _activeEnemies.RemoveAt(i--);
            }
        }
    }

    private void ClampPosition(EntityBase entity) {
        Vector2 pos = entity.transform.position;
        pos.x = Mathf.Clamp(pos.x, _stageMinSize.x + entity.Radius, _stageMaxSize.x - entity.Radius);
        pos.y = Mathf.Clamp(pos.y, _stageMinSize.y + entity.Radius, _stageMaxSize.y - entity.Radius);
        entity.transform.position = pos;
    }

    public void OnEntityActive(EntityBase entity) {
        entity.gameObject.SetActive(true);
        _inactiveAllies.Remove(entity);
        _activeAllies.Add(entity);
    }

    public void OnEntityInactive(EntityBase entity) {
        _inactiveAllies.Add(entity);
        entity.gameObject.SetActive(false);
    }

    private void SpawnEnemy(int amount) {
        for (int i = 0; i < amount; ++i) {
            EntityBase enemyPrefab = _enemyPrefabMelee;
            if (Random.Range(0, 2) > 0) {
                enemyPrefab = _enemyPrefabRange;
            }

            float randX = Random.Range(_stageMinSize.x, _stageMaxSize.x);
            float y = Random.Range(0, 2) > 0 ? _stageMaxSize.y + enemyPrefab.Radius : _stageMinSize.y - enemyPrefab.Radius;
            Vector3 randPos = new Vector3(randX, y);

            EntityBase enemy = Instantiate(enemyPrefab, randPos, Quaternion.identity);
            enemy.Initialize();
            _activeEnemies.Add(enemy);
        }
    }
}
