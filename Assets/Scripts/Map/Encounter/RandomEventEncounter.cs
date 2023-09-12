using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventEncounter : EncounterBase {
    [SerializeField] private RandomEventHandler _randomeEncounterHandler = null;
    private void Awake() {
        
    }

    public override void OnEncounter() {
        _randomeEncounterHandler.Initialize();
        gameObject.SetActive(true);
    }

    public override void Reset() {
        _randomeEncounterHandler.Reset();
    }
}