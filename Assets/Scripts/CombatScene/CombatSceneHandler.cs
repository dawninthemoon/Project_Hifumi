using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class CombatSceneHandler : MonoBehaviour, IResetable {
    [SerializeField] private float _targetDetectionDelay = 0.05f;
    [SerializeField] private Truck _truck = null;
    [SerializeField] private AllyHandler _allyHandler = null;
    [SerializeField] private EnemyHandler _enemyHandler = null;
    [SerializeField] private CombatReward _combatReward = null;
    [SerializeField] private CombatResultUI _combatResultUI = null;
    private CombatStageConfig _currentStageConfig;
    private EntitySpawner _entitySpawner;
    private UnityEvent _onStageEnd;
    private int _currentWave = 0;
    private ExTimeCounter _timeCounter;
    private bool _waitingForNextWave;
    private bool _isStageCleared;
    private float _stageTimeAgo;
    private float _targetDetectionCounter;
    private static readonly string NextWaveTimerKey = "NextWaveTime";
    public float NextWaveTime {
        get {
            bool isLastStage = (_currentWave == _currentStageConfig.GetStageLength());
            if (_isStageCleared || isLastStage || !_timeCounter.Contains(NextWaveTimerKey))
                return 0;
            float timeLimit = _timeCounter.GetTimeLimit(NextWaveTimerKey);
            float curr = _timeCounter.GetCurrentTime(NextWaveTimerKey);
            return Mathf.Max(0f, timeLimit - curr);
        }
    }
    public bool IsCombatStarted {
        get { return _truck && _truck.MoveProgressEnd; }
    }
    
    private void Awake() {
        _timeCounter = new ExTimeCounter();
        _entitySpawner = new EntitySpawner(transform);
        _onStageEnd = new UnityEvent();
        _onStageEnd.AddListener(OnStageEnd);
    }

    public void Reset() {
        _stageTimeAgo = 0f;
        _allyHandler.RemoveAllAllies(_entitySpawner);
        _enemyHandler.RemoveAllEnemies(_entitySpawner);
        _combatResultUI.Reset();

        _currentWave = 0;
        _targetDetectionCounter = 0f;
        _isStageCleared = false;
        _waitingForNextWave = true;
    }

    public void StartCombat(CombatStageConfig stageConfig) {
        _currentStageConfig = stageConfig;

        _allyHandler.SetTruckObject(_truck);
        _allyHandler.InitalizeAllies(_entitySpawner);
        InitializeCombat();
    }

    private void InitializeCombat() {
        CombatMap.SetMapView(Vector3.zero);
        StartNewWave();
    }

    private void Update() {
        if (_isStageCleared || !IsCombatStarted) {
            return;
        }
        _stageTimeAgo += Time.deltaTime;

        TargetDetectProgress();

        if (!_waitingForNextWave && _enemyHandler.ActiveEnemies.Count == 0) {
            OnWaveCleared();
        }

        if (!_isStageCleared && _timeCounter.Contains(NextWaveTimerKey)) {
            _timeCounter.IncreaseTimer(NextWaveTimerKey, out var limit, GameConfigHandler.GameSpeed);
            if (limit) {
                StartNewWave();
            }
        }
    }

    private void TargetDetectProgress() {
        _targetDetectionCounter -= Time.deltaTime;
        if (_targetDetectionCounter < 0f) {
            _targetDetectionCounter = _targetDetectionDelay;

            _allyHandler.Progress(_enemyHandler);
            _enemyHandler.Progress(_allyHandler.ActiveAllies, _truck);
        }
    }

    public void StartNewWave() {
        if (_currentWave + 1 <= _currentStageConfig.GetStageLength()) {
            ++_currentWave;
            _waitingForNextWave = false;
            _timeCounter.InitTimer(NextWaveTimerKey, 0f, 30f);
            _enemyHandler.SpawnEnemies(_currentWave, _currentStageConfig, _entitySpawner);
        }
    }

    private void OnWaveCleared() {
        if (_currentWave == _currentStageConfig.GetStageLength()) {
            _onStageEnd.Invoke();
        }
        else {
            _waitingForNextWave = true;
        }
    }

    private void OnStageEnd() {
        _isStageCleared = true;

        _allyHandler.DisarmAllAllies();

        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Entity, false);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.UI, false);

        _combatReward.OpenRewardSet(OnRewardSelected);
    }

    private void OnRewardSelected(Belongings selectedStuff) {
        GameMain.PlayerData.AddBelongingsInInventory(selectedStuff);

        _combatResultUI.ShowResultUI(true, Mathf.FloorToInt(_stageTimeAgo).ToString());

        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Entity, true);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.UI, true);
    }
}
