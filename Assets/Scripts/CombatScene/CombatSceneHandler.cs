using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class CombatSceneHandler : MonoBehaviour, IResetable {
    [SerializeField] private MemberUIControl _memberUIControl = null;
    [SerializeField] private AllyHandler _allyHandler = null;
    [SerializeField] private EnemyHandler _enemyHandler = null;
    [SerializeField] private CombatReward _combatReward = null;
    [SerializeField] private CombatResultUI _combatResultUI = null;
    private CombatStageConfig _currentStageConfig;
    private EntitySpawner _entitySpawner;
    private Truck _truck;
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
        _onStageEnd = new UnityEvent();
        _onStageEnd.AddListener(OnStageEnd);
    }

    public void Reset() {
        _timeAgo = 0f;
        _allyHandler.RemoveAllAllies(_entitySpawner);
        _enemyHandler.RemoveAllEnemies(_entitySpawner);
        _combatResultUI.Reset();
        _memberUIControl.Reset();

        _currentWave = 0;
        _isStageCleared = false;
        _waitingForNextWave = true;
    }

    public void StartCombat(CombatStageConfig stageConfig) {
        _currentStageConfig = stageConfig;

        _truck = _memberUIControl.GetComponent<Truck>();
        _allyHandler.SetTruckObject(_truck);
        _allyHandler.InitalizeAllies(_entitySpawner);
        _memberUIControl.InitializeEntityUI(_allyHandler.OnEntityActive, _allyHandler.OnEntityInactive, _allyHandler.InactiveAllies);
        InitializeCombat();
    }

    private void InitializeCombat() {
        CombatMap.SetMapView(Vector3.zero);
        StartNewWave();
    }

    private void Update() {
        if (_isStageCleared) {
            return;
        }
        _timeAgo += Time.deltaTime;

        MoveProgress();
        foreach (EntityBase inactiveAlly in _allyHandler.InactiveAllies) {
            _memberUIControl.UpdateMemberElement(inactiveAlly);
        }

        
        if (_truck.MoveProgressEnd && !_allyHandler.HasActiveAlly) {
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
        _allyHandler.Progress(_enemyHandler);
        _enemyHandler.Progress(_allyHandler.ActiveAllies, _truck);
    }

    public void StartNewWave() {
        if (_enemyHandler.ActiveEnemies.Count > 0)
            return;

        if (++_currentWave <= _currentStageConfig.StageInfoArray.Length) {
            _waitingForNextWave = false;
            
            _enemyHandler.SpawnEnemies(_currentWave, _currentStageConfig, _entitySpawner);
        }
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

        _allyHandler.DisarmAllAllies();

        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Entity, false);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.UI, false);

        _combatReward.OpenRewardSet(OnRewardSelected);
    }

    private void OnRewardSelected(Belongings selectedStuff) {
        GameMain.PlayerData.AddBelongingsInInventory(selectedStuff);

        _combatResultUI.ShowResultUI(true, _timeAgo.ToString());

        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Entity, true);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.UI, true);
    }
}
