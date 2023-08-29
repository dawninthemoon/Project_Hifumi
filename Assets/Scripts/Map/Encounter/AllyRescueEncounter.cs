using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyRescueEncounter : EncounterBase {
    [SerializeField] private AllyRescueHandler _allyRescueHandler = null;
    private void Awake() {

    }

    public override void OnEncounter() {
        _allyRescueHandler.Reset();
        gameObject.SetActive(true);
    }

    public override void Reset() {
        
    }
}