using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class GameTest : MonoBehaviour {
    [SerializeField] private MemberUIControl _memberUIControl = null;
    [SerializeField, Range(0.5f, 10f)] private float _timeScale = 1f;
    [SerializeField] private int _entityCount = 2;
    [SerializeField] private int _waveCount = 3;
    [SerializeField] private UnityEvent _onStageEnd = null;
    private KdTree<EntityBase> _activeAllies;
    private KdTree<EntityBase> _activeEnemies;
    private List<EntityBase> _inactiveAllies;
    public static readonly float Width = 640;
    public static readonly float Height = 380f;
    private static Vector2 _stageMinSize;
    private static Vector2 _stageMaxSize;
    public static Vector2 StageMinSize { get { return _stageMinSize; } }
    public static Vector2 StageMaxSize { get { return _stageMaxSize; } }
    private int _currentWave = 0;
    private float _gameSpeed;

    private void Awake() {
        _activeAllies = new KdTree<EntityBase>(true);
        _activeEnemies = new KdTree<EntityBase>(true);
        _inactiveAllies = new List<EntityBase>();
        _onStageEnd.AddListener(() => InteractiveEntity.IsInteractive = false);
        InitalizeEntities();
    }

    private void Start() {
        _memberUIControl.InitializeEntityUI(OnEntityActive, OnEntityInactive, _activeAllies.ToList());
        InitializeCombat();
    }

    public static void SetMapView(Vector3 origin) {
        _stageMinSize = (Vector2)origin + new Vector2(-Width / 2f, -Height / 2f);
        _stageMaxSize = (Vector2)origin + new Vector2(Width / 2f, Height / 2f);
    }

    private void InitializeCombat() {
        InteractiveEntity.IsInteractive = true;
        SetMapView(Vector3.zero);
    
        StartNewWave(_entityCount);
    }

    private void InitalizeEntities() {
        var entityPrefab = Resources.Load<EntityBase>("Prefabs/AllyPrefab");
        var entityInformation = Resources.LoadAll<EntityInfo>("ScriptableObjects/Allies");
        foreach (EntityInfo info in entityInformation) {
            float radius = 100f;
            Vector3 randomPos = Random.insideUnitCircle.normalized * radius;

            EntityBase newEntity = Instantiate(entityPrefab, randomPos, Quaternion.identity);
            newEntity.Initialize(info);
            
            _activeAllies.Add(newEntity);
        }
    }

    private void Update() {
        Time.timeScale = _timeScale;
        if (Input.GetKeyDown(KeyCode.X)) {
            _gameSpeed = Mathf.Max(0.5f, _gameSpeed - 0.5f);
            Time.timeScale = _timeScale = _gameSpeed;
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            _gameSpeed = Mathf.Min(10f, _gameSpeed + 0.5f);
            Time.timeScale = _timeScale = _gameSpeed;
        }

        MoveProgress();
        
        if (_activeAllies.Count == 0) {
            _memberUIControl.gameObject.layer = LayerMask.NameToLayer("Ally");
        }
        else {
            _memberUIControl.gameObject.layer = LayerMask.NameToLayer("Obstacle");
        }

        if (_activeEnemies.Count == 0) {
            StartNewWave(_entityCount);
        }
    }

    private void MoveProgress() {
        foreach (EntityBase ally in _activeAllies) {
            ITargetable target = _activeEnemies.FindClosest(ally.transform.position)?.GetComponent<Agent>();
            ally.SetTarget(target);

            ClampPosition(ally);
        }

        foreach (EntityBase enemy in _activeEnemies) {
            ITargetable target = _activeAllies.FindClosest(enemy.transform.position)?.GetComponent<Agent>();
            if (target == null) {
                //target = _memberUIControl.GetComponent<Truck>();
            }

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
        entity.SetTarget(null);
        entity.gameObject.SetActive(false);
    }

    private void StartNewWave(int amount) {
        if (++_currentWave > _waveCount) {
            _onStageEnd.Invoke();
            return;
        }

        EntityBase enemyPrefab = Resources.Load<EntityBase>("Prefabs/EnemyPrefab");
        var entityInformation = Resources.LoadAll<EntityInfo>("ScriptableObjects/Enemies");
        for (int i = 0; i < amount; ++i) {
            int randomIndex = Random.Range(0, entityInformation.Length);

            float randX = Random.Range(_stageMinSize.x, _stageMaxSize.x);
            float y = Random.Range(0, 2) > 0 ? _stageMaxSize.y + enemyPrefab.Radius : _stageMinSize.y - enemyPrefab.Radius;
            
            if (_currentWave == 1) {
                y = Random.Range(_stageMinSize.y / 4f, _stageMaxSize.y / 4f);
            }
            
            Vector3 randPos = new Vector3(randX, y);

            EntityBase enemy = Instantiate(enemyPrefab, randPos, Quaternion.identity);
            enemy.Initialize(entityInformation[randomIndex]);
            _activeEnemies.Add(enemy);
        }
    }
}
