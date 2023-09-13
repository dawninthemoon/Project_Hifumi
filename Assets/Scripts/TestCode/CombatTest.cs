using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class CombatTest : MonoBehaviour {
    [SerializeField] private CombatEncounter _combatEncounter = null;

    private void Start() {
        StartCoroutine(StartEncounter());
    }

    private IEnumerator StartEncounter() {
        Debug.Log("Stage Loading...");
        yield return new WaitUntil(() => IsStageLoadedCompleted());
        Debug.Log("Loaded Completed!");
        _combatEncounter.OnEncounter();
    }

    private void Update() {
        if (IsStageLoadedCompleted() && Input.GetKeyDown(KeyCode.Z)) {
            Camera.main.transform.position = Vector3.zero.ChangeZPos(-10f);
            _combatEncounter.Reset();
            _combatEncounter.OnEncounter();
        }
    }

    private bool IsStageLoadedCompleted() {
        return EntitySpawner.IsLoadCompleted 
                && EnemyHandler.IsLoadCompleted 
                && CombatEncounter.IsLoadCompleted;
    }
}
