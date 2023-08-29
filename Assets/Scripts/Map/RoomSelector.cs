using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class RoomSelector : MonoBehaviour {
    [SerializeField] private GameObject _gameMapParent = null;
    [SerializeField] private CombatEncounter _combatEncounter = null;
    [SerializeField] private ShopEncounter _shopEncounter = null;
    [SerializeField] private AllyRescueEncounter _allyRescueEncounter = null;
    private GameObject _currentRoomParent;
    private System.Action _requestedRoomExitCallback;

    private void Awake() {
        _combatEncounter = Instantiate(_combatEncounter, Vector3.zero, Quaternion.identity);
        _shopEncounter = Instantiate(_shopEncounter, Vector3.zero, Quaternion.identity);
        _allyRescueEncounter = Instantiate(_allyRescueEncounter, Vector3.zero, Quaternion.identity);
    }

    private void Start() {
        _currentRoomParent = _gameMapParent;
        _combatEncounter.gameObject.SetActive(false);
        _shopEncounter.gameObject.SetActive(false);
        _allyRescueEncounter.gameObject.SetActive(false);
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
            target = _shopEncounter;
            break;
        case EncounterType.ALLY:
            target = _allyRescueEncounter;
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
        Camera.main.transform.position = Vector3.zero.ChangeZPos(-10f);
        encounter.OnEncounter();
    }

    public void ExitRoom() {
        ChangeRoomSetting(_gameMapParent);
        _requestedRoomExitCallback.Invoke();
    }

    private void ChangeRoomSetting(GameObject target) {
        _currentRoomParent.SetActive(false);
        _currentRoomParent = target;
        target.SetActive(true);
    }
}    
