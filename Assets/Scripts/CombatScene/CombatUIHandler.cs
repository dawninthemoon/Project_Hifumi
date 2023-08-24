using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUIHandler : MonoBehaviour {
    [SerializeField] private Button _timerButton = null;
    private TextMeshProUGUI _timerText;
    private CombatSceneHandler _combatScene;

    void Awake() {
        _timerText = _timerButton.GetComponentInChildren<TextMeshProUGUI>();
        _combatScene = GameObject.FindObjectOfType<CombatSceneHandler>();
    }

    void Update() {
        _timerText.text = Mathf.Floor(_combatScene.NextWaveTime).ToString();
    }
}
