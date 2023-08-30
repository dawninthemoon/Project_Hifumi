using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class CombatSceneHandler : MonoBehaviour, IResetable {
    [SerializeField] private MemberUIControl _memberUIControl = null;
    [SerializeField] private EnemyHandler _enemyHandler = null;
    [SerializeField] private CombatReward _combatReward = null;
    [SerializeField] private CombatResultUI _combatResultUI = null;
    public static readonly float Width = 640;
    public static readonly float Height = 380f;
    private KdTree<EntityBase> _activeAllies;
    private List<EntityBase> _inactiveAllies;
    private CombatStageConfig _currentStageConfig;
    private EntitySpawner _entitySpawner;
    private Truck _truck;
    private static Vector2 _stageMinSize;
    private static Vector2 _stageMaxSize;
    public static Vector2 StageMinSize { get { return _stageMinSize; } }
    public static Vector2 StageMaxSize { get { return _stageMaxSize; } }
    private UnityEvent _onStageEnd;
    private int _currentWave = 0;
    private ExTimeCounter _timeCounter;
    private bool _waitingForNextWave;
    private bool _isStageCleared;
    private float _timeAgo;
    private static readonly string NextWaveTimerKey = "NextWaveTime";
    public float NextWaveTime {
        get {
            if (!_waitingForNextWave || !_timeCounter.Contains(NextWaveTimerKey))
                return 0;
            float timeLimit = _timeCounter.GetTimeLimit(NextWaveTimerKey);
            float curr = _timeCounter.GetCurrentTime(NextWaveTimerKey);
            return timeLimit - curr;
        }
    }
    
    private void Awake() {
        _timeCounter = new ExTimeCounter();
        _entitySpawner = new EntitySpawner(transform);
        _activeAllies = new KdTree<EntityBase>(true);
        _inactiveAllies = new List<EntityBase>();
        _onStageEnd = new UnityEvent();
        _onStageEnd.AddListener(OnStageEnd);
    }

    public void Reset() {
        _timeAgo = 0f;
        for (int i = 0; i < _activeAllies.Count; ++i) {
            _entitySpawner.RemoveAlly(_activeAllies[i]);
            _activeAllies.RemoveAt(i--);
        }
        for (int i = 0; i < _inactiveAllies.Count; ++i) {
            _entitySpawner.RemoveAlly(_inactiveAllies[i]);
            _inactiveAllies.RemoveAt(i--);
        }
        _enemyHandler.RemoveAllEnemies(_entitySpawner);
        _combatResultUI.Reset();

        _currentWave = 0;
        _isStageCleared = false;
        _waitingForNextWave = true;
    }

    public void StartCombat(CombatStageConfig stageConfig) {
        _currentStageConfig = stageConfig;

        InitalizeAllies();
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
        _truck = _memberUIControl.GetComponent<Truck>();

        foreach (EntityInfo info in GameMain.PlayerData.Allies) {
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
        _timeAgo += Time.deltaTime;

        MoveProgress();
        
        if (_truck.MoveProgressEnd && _activeAllies.Count == 0) {
            _memberUIControl.gameObject.layer = LayerMask.NameToLayer("Ally");
        }
        else {
            _memberUIControl.gameObject.layer = LayerMask.NameToLayer("Obstacle");
        }

        if (!_waitingForNextWave && _enemyHandler.ActiveEnemies.Count == 0) {
            OnWaveCleared();
        }

        if (_waitingForNextWave && _timeCounter.Contains(NextWaveTimerKey)) {
            _timeCounter.IncreaseTimer(NextWaveTimerKey, out var limit, GameConfigHandler.GameSpeed);
            if (limit) {
                StartNewWave();
            }
        }
    }

    private void MoveProgress() {
        foreach (EntityBase ally in _activeAllies) {
            var activeEnemies = _enemyHandler.ActiveEnemies;
            ITargetable target = activeEnemies.FindClosest(ally.transform.position)?.GetComponent<Agent>();
            ally.SetTarget(target);

            ClampPosition(ally);
        }

        _enemyHandler.Progress(_activeAllies, _truck);
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

    public void OnEntityInactive(EntityBase entity) {
        _inactiveAllies.Add(entity);
        entity.SetTarget(null);
        entity.gameObject.SetActive(false);
    }

    public void StartNewWave() {
        if (_enemyHandler.ActiveEnemies.Count > 0)
            return;

        ++_currentWave;
        _waitingForNextWave = false;
        
        _enemyHandler.SpawnEnemies(_currentWave, _currentStageConfig, _entitySpawner);
    }

    private void OnWaveCleared() {
        if (_currentWave == _currentStageConfig.GetStageLength()) {
            _onStageEnd.Invoke();
        }
        else {
            _timeCounter.InitTimer(NextWaveTimerKey, 0f, 20f);
            _waitingForNextWave = true;
        }
    }

    private void OnStageEnd() {
        _isStageCleared = true;

        for (int i = 0; i < _activeAllies.Count; ++i) {
            _activeAllies[i].SetTarget(null);
        }

        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Entity, false);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.UI, false);

        _combatReward.OpenRewardSet(OnRewardSelected);
    }

    private void OnRewardSelected(Belongings selectedStuff) {
        GameMain.PlayerData.AddBelongings(selectedStuff);

        _combatResultUI.ShowResultUI(true, _timeAgo.ToString());

        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Entity, true);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.UI, true);
    }
}
