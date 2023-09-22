using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class AllyRescueTest : MonoBehaviour {
    [SerializeField] private AllyRescueEncounter _rescueEncounter = null;

    private void Start() {
        StartCoroutine(StartEncounter());
    }

    private IEnumerator StartEncounter() {
        Debug.Log("Scene Loading...");
        yield return new WaitUntil(() => IsStageLoadedCompleted());
        Debug.Log("Loaded Completed!");
        _rescueEncounter.OnEncounter();
    }

    private void Update() {
        if (IsStageLoadedCompleted() && Input.GetKeyDown(KeyCode.Z)) {
            _rescueEncounter.Reset();
            _rescueEncounter.OnEncounter();
        }
    }

    private bool IsStageLoadedCompleted() {
        return GameMain.IsLoadCompleted;
    }
}
