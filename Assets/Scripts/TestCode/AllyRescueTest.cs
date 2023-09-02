using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class AllyRescueTest : MonoBehaviour {
    [SerializeField] private AllyRescueEncounter _rescueEncounter = null;

    private void Start() {
        _rescueEncounter.OnEncounter();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            _rescueEncounter.Reset();
            _rescueEncounter.OnEncounter();
        }
    }
}
