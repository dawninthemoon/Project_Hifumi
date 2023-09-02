using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTest : MonoBehaviour {
    [SerializeField] private ShopEncounter _shopEncounter = null;

    private void Start() {
        _shopEncounter.OnEncounter();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            _shopEncounter.Reset();
            _shopEncounter.OnEncounter();
        }
    }
}
