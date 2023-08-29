using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class CombatTest : MonoBehaviour {
    [SerializeField] private CombatEncounter _combatEncounter = null;

    private void Start() {
        _combatEncounter.OnEncounter();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            Camera.main.transform.position = Vector3.zero.ChangeZPos(-10f);
            _combatEncounter.Reset();
            _combatEncounter.OnEncounter();
        }
    }
}
