using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEncounter : EncounterBase {
    [SerializeField] private ShopHandler _shopHandler = null;
    private void Awake() {

    }

    public override void OnEncounter() {
        _shopHandler.Reset();
        gameObject.SetActive(true);
    }
}