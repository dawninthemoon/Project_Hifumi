using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEncounter : EncounterBase {
    [SerializeField] private RandomEncounterHandler _randomeEncounterHandler = null;
    private void Awake() {
        
    }

    public override void OnEncounter() {
        _randomeEncounterHandler.Reset();
        gameObject.SetActive(true);
    }

    public override void Reset() {
        _randomeEncounterHandler.Reset();
    }
}