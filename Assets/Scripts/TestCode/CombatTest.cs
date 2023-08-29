using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTest : MonoBehaviour {
    [SerializeField] private CombatEncounter _combatEncounter = null;

    private void Start() {
        _combatEncounter.OnEncounter();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            _combatEncounter.Reset();
            _combatEncounter.OnEncounter();
        }
    }
}
