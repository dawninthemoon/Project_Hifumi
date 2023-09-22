using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class RoomSelector : MonoBehaviour {
    [SerializeField] private GameObject _gameMapParent = null;
    [SerializeField] private CombatEncounter _combatEncounter = null;
    [SerializeField] private ShopEncounter _shopEncounter = null;
    [SerializeField] private AllyRescueEncounter _allyRescueEncounter = null;
    [SerializeField] private RandomEventEncounter _eventEncounter = null;
    private EncounterBase _currentEncounter;
    private GameObject _currentEncounterObj;
    private System.Action _requestedRoomExitCallback;

    private void Awake() {
        _combatEncounter = Instantiate(_combatEncounter, Vector3.zero, Quaternion.identity);
        _shopEncounter = Instantiate(_shopEncounter, Vector3.zero, Quaternion.identity);
        _allyRescueEncounter = Instantiate(_allyRescueEncounter, Vector3.zero, Quaternion.identity);
        _eventEncounter = Instantiate(_eventEncounter, Vector3.zero, Quaternion.identity);
    }

    private void Start() {
        _currentEncounterObj = _gameMapParent;

        _combatEncounter.Initialize(ExitRoom);
        _shopEncounter.Initialize(ExitRoom);
        _allyRescueEncounter.Initialize(ExitRoom);
        _eventEncounter.Initialize(ExitRoom);
    }

    public void SetRoomExit(System.Action callback) {
        _requestedRoomExitCallback = callback;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            ExitRoom();
        }
    }

    public void StartEnterRoom(EncounterType encounterType) {
        switch (encounterType) {
        case EncounterType.COMBAT:
            _currentEncounter = _combatEncounter;
            break;
        case EncounterType.SHOP:
            _currentEncounter = _shopEncounter;
            break;
        case EncounterType.ALLY:
            _currentEncounter = _allyRescueEncounter;
            break;
        case EncounterType.EVENT:
            _currentEncounter = _eventEncounter;
            break;
        }
        EnterRoom(_currentEncounter);
    }

    private void EnterRoom(EncounterBase encounter) {
        if (encounter == null) {
            Debug.LogError("Room Not Exists");
            return;
        }
        ChangeRoomSetting(encounter.gameObject);
        Camera.main.transform.position = Vector3.zero.ChangeZPos(-10f);
        encounter.OnEncounter();
    }

    public void ExitRoom() {
        ChangeRoomSetting(_gameMapParent);
        _currentEncounter.Reset();
        _requestedRoomExitCallback.Invoke();
    }

    private void ChangeRoomSetting(GameObject target) {
        _currentEncounterObj?.SetActive(false);
        _currentEncounterObj = target;
        target.SetActive(true);
    }
}    
