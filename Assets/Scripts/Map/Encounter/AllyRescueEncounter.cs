using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AllyRescueEncounter : EncounterBase {
    [SerializeField] private AllyRescueHandler _allyRescueHandler = null;
    private void Awake() {

    }

    public override void Initialize(UnityAction roomExitCallback) {
        gameObject.SetActive(false);
    }

    public override void OnEncounter() {
        _allyRescueHandler.InitializeAllies();
        gameObject.SetActive(true);
    }

    public override void Reset() {
        _allyRescueHandler.Reset();
    }
}