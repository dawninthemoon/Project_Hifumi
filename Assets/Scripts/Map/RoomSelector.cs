using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSelector : MonoBehaviour {
    [SerializeField] private GameObject _gameMapParent = null;
    [SerializeField] private CombatEncounter _combatEncounter = null;
    private GameObject _currentRoomParent;
    private System.Action _requestedRoomExitCallback;

    private void Start() {
        _currentRoomParent = _gameMapParent;
        _combatEncounter.gameObject.SetActive(false);
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
        EncounterBase target = null;
        switch (encounterType) {
        case EncounterType.COMBAT:
            target = _combatEncounter;
            break;
        case EncounterType.SHOP:
            //target = _battleRoomEncounter;
            break;
        case EncounterType.ALLY:
            //target = _shopRoomEncounter;
            break;
        }
        EnterRoom(target);
    }

    private void EnterRoom(EncounterBase encounter) {
        if (encounter == null) {
            Debug.LogError("Room Not Exists");
            return;
        }
        ChangeRoomSetting(encounter.gameObject);
        encounter.OnEncounter();
    }

    public void ExitRoom() {
        ChangeRoomSetting(_gameMapParent);
        //_requestedRoomExitCallback.Invoke();
    }

    private void ChangeRoomSetting(GameObject target) {
        _currentRoomParent.SetActive(false);
        _currentRoomParent = target;
        target.SetActive(true);
    }
}    
