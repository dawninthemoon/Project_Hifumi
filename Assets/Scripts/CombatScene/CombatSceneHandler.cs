using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class CombatSceneHandler : MonoBehaviour {
    [SerializeField] private MemberUIControl _memberUIControl = null;
    [SerializeField, Range(0.5f, 10f)] private float _timeScale = 1f;
    [SerializeField] private int _entityCount = 2;
    [SerializeField] private int _waveCount = 3;
    [SerializeField] private UnityEvent _onStageEnd = null;
    public static readonly float Width = 640;
    public static readonly float Height = 380f;
    private KdTree<EntityBase> _activeAllies;
    private KdTree<EntityBase> _activeEnemies;
    private List<EntityBase> _inactiveAllies;
    private EntitySpawner _entitySpawner;
    private Truck _truck;
    private static Vector2 _stageMinSize;
    private static Vector2 _stageMaxSize;
    public static Vector2 StageMinSize { get { return _stageMinSize; } }
    public static Vector2 StageMaxSize { get { return _stageMaxSize; } }
    private int _currentWave = 0;
    private ExTimeCounter _timeCounter;
    private bool _waitingForNextWave;
    private bool _isStageCleared;
    public float NextWaveTime {
        get {
            if (!_waitingForNextWave || !_timeCounter.Contains("NextWaveTime"))
                return 0;
            float timeLimit = _timeCounter.GetTimeLimit("NextWaveTime");
            float curr = _timeCounter.GetCurrentTime("NextWaveTime");
            return timeLimit - curr;
        }
    }
    private float _gameSpeed;

    private void Awake() {
        _timeCounter = new ExTimeCounter();
        _entitySpawner = new EntitySpawner();
        
        _activeAllies = new KdTree<EntityBase>(true);
        _activeEnemies = new KdTree<EntityBase>(true);
        _inactiveAllies = new List<EntityBase>();
        _onStageEnd.AddListener(OnStageEnd);
        InitalizeAllies();
    }

    private void Start() {
        _memberUIControl.InitializeEntityUI(OnEntityActive, OnEntityInactive, _inactiveAllies);
        InitializeCombat();
    }

    public static void SetMapView(Vector3 origin) {
        _stageMinSize = (Vector2)origin + new Vector2(-Width / 2f, -Height / 2f);
        _stageMaxSize = (Vector2)origin + new Vector2(Width / 2f, Height / 2f);
    }

    private void InitializeCombat() {
        SetMapView(Vector3.zero);
    
        StartNewWave();
    }

    private void InitalizeAllies() {
        var entityPrefab = Resources.Load<EntityBase>("Prefabs/AllyPrefab");
        var entityInformation = Resources.LoadAll<EntityInfo>("ScriptableObjects/Allies");
        _truck = _memberUIControl.GetComponent<Truck>();

        foreach (EntityInfo info in entityInformation) {
            EntityBase newEntity = _entitySpawner.CreateAlly(info);
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

    private void Update() {
        if (_isStageCleared) {
            return;
        }

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
        
        if (_truck.MoveProgressEnd && _activeAllies.Count == 0) {
            _memberUIControl.gameObject.layer = LayerMask.NameToLayer("Ally");
        }
        else {
            _memberUIControl.gameObject.layer = LayerMask.NameToLayer("Obstacle");
        }

        if (!_waitingForNextWave && _activeEnemies.Count == 0) {
            OnWaveCleared();
        }

        if (_waitingForNextWave && _timeCounter.Contains("NextWaveTime")) {
            _timeCounter.IncreaseTimer("NextWaveTime", out var limit);
            if (limit) {
                StartNewWave();
            }
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
            if (target == null && _truck.MoveProgressEnd) {
                target = _truck;
            }

            enemy.SetTarget(target);
            
            if (_truck.MoveProgressEnd)
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
                _activeAllies[i].SetTarget(null);
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
        StartCoroutine(StartEaseParabola(entity.transform, () => _activeAllies.Add(entity)));
    }

    private IEnumerator StartEaseParabola(Transform target, System.Action callback) {
        float timeAgo = 0f;
        float targetTime = 1f;

        float radius = 100f;
        Vector2 start = _truck.transform.position;
        Vector2 end = start + Random.insideUnitCircle.normalized * radius;
        Vector2 p1 = _truck.Position;
        p1.y += 100f;

        Time.timeScale = _gameSpeed * 0.25f;
        while (timeAgo < targetTime) {
            timeAgo += Time.deltaTime;

            Vector2 p = Bezier.GetPoint(start, p1, end, timeAgo / targetTime);
            target.position = p;

            yield return null;
        }
        Time.timeScale = _gameSpeed;

        callback();
    }

    public void OnEntityInactive(EntityBase entity) {
        _inactiveAllies.Add(entity);
        entity.SetTarget(null);
        entity.gameObject.SetActive(false);
    }

    private void OnWaveCleared() {
        if (_currentWave == _waveCount) {
            _onStageEnd.Invoke();
        }
        else {
            _timeCounter.InitTimer("NextWaveTime", 0f, 20f);
            _waitingForNextWave = true;
        }
    }

    private void OnStageEnd() {
        _isStageCleared = true;
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Entity, false);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.UI, false);
    }

    public void StartNewWave() {
        if (_activeEnemies.Count > 0)
            return;

        ++_currentWave;
        _waitingForNextWave = false;

        int amount = _entityCount;
        EntityBase enemyPrefab = Resources.Load<EntityBase>("Prefabs/EnemyPrefab");
        var entityInformation = Resources.LoadAll<EntityInfo>("ScriptableObjects/Enemies");
        for (int i = 0; i < amount; ++i) {
            int randomIndex = Random.Range(0, entityInformation.Length);

            float randX = Random.Range(_stageMinSize.x, _stageMaxSize.x);
            float y = Random.Range(0, 2) > 0 ? _stageMaxSize.y + enemyPrefab.Radius : _stageMinSize.y - enemyPrefab.Radius;
            if (_currentWave == 1) {
                y = Random.Range(_stageMinSize.y / 4f, _stageMaxSize.y / 4f);
            }
            
            EntityBase enemy = _entitySpawner.CreateEnemy(entityInformation[randomIndex]);
            enemy.transform.position = new Vector3(randX, y);

            _activeEnemies.Add(enemy);
        }
    }
}
