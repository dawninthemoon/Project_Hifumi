using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEncounter : EncounterBase {
    [SerializeField] private ShopHandler _shopHandler = null;
    private void Awake() {

    }

    public override void OnEncounter() {
        gameObject.SetActive(true);
    }

    public override void Reset() {
        _shopHandler.Reset();
    }
}