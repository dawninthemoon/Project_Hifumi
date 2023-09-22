using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventTest : MonoBehaviour {
    [SerializeField] private RandomEventEncounter _eventEncounter = null;

    private void Start() {
        StartCoroutine(StartEncounter());
    }

    private IEnumerator StartEncounter() {
        Debug.Log("Event Loading...");
        yield return new WaitUntil(() => IsStageLoadedCompleted());
        Debug.Log("Loaded Completed!");
        _eventEncounter.OnEncounter();
    }

    private void Update() {
        if (IsStageLoadedCompleted() && Input.GetKeyDown(KeyCode.Z)) {
            _eventEncounter.Reset();
            _eventEncounter.OnEncounter();
        }
    }

    private bool IsStageLoadedCompleted() {
        return RandomEventHandler.IsLoadCompleted;
    }
}
