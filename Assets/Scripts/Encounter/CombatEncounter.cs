using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEncounter : EncounterBase {
    [SerializeField] private CombatSceneHandler _combatHandler = null;
    [SerializeField] private TruckMover _truckMover = null;

    public override void OnEncounter() {
        _combatHandler.StartCombat();
        _truckMover.Reset();

        gameObject.SetActive(true);
    }
}
