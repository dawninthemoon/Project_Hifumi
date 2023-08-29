using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatTimerUI : MonoBehaviour {
    [SerializeField] private Canvas _combatUICanvas = null;
    [SerializeField] private Button _timerButton = null;
    private TextMeshProUGUI _timerText;
    private CombatSceneHandler _combatScene;

    void Awake() {
        _combatUICanvas.worldCamera = Camera.main;
        _timerText = _timerButton.GetComponentInChildren<TextMeshProUGUI>();
        _combatScene = GameObject.FindObjectOfType<CombatSceneHandler>();
    }

    void Update() {
        _timerText.text = Mathf.Floor(_combatScene.NextWaveTime).ToString();
    }
}
