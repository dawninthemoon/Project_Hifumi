using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopEncounter : EncounterBase {
    [SerializeField] private ShopHandler _shopHandler;
    [SerializeField] private Canvas _shopUICanvas;
    private void Awake() {
        _shopUICanvas.worldCamera = Camera.main;
    }

    public override void Initialize(Action roomExitCallback) {
        gameObject.SetActive(false);
    }

    public override void OnEncounter() {
        gameObject.SetActive(true);
    }

    public override void Reset() {
        _shopHandler.Reset();
    }
}