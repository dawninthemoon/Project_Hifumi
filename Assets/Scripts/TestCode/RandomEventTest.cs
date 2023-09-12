using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventTest : MonoBehaviour {
    [SerializeField] private RandomEventEncounter _eventEncounter = null;

    private void Start() {
        _eventEncounter.OnEncounter();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            _eventEncounter.Reset();
            _eventEncounter.OnEncounter();
        }
    }
}
