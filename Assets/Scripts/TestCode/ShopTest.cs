using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTest : MonoBehaviour {
    [SerializeField] private ShopEncounter _shopEncounter = null;

    private void Start() {
        StartCoroutine(StartEncounter());
    }

    private IEnumerator StartEncounter() {
        Debug.Log("Shop Loading...");
        yield return new WaitUntil(() => IsStageLoadedCompleted());
        Debug.Log("Loaded Completed!");
        _shopEncounter.OnEncounter();
    }

    private void Update() {
        if (IsStageLoadedCompleted() && Input.GetKeyDown(KeyCode.Z)) {
            _shopEncounter.Reset();
            _shopEncounter.OnEncounter();
        }
    }

    private bool IsStageLoadedCompleted() {
        return GameMain.IsLoadCompleted;
    }
}
