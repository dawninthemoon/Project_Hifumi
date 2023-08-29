using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatResultUI : MonoBehaviour, IResetable {
    [SerializeField] private GameObject _resultUIObject = null;
    [SerializeField] private TextMeshProUGUI _titleText = null;
    [SerializeField] private TextMeshProUGUI _timeText = null;
    [SerializeField] private TextMeshProUGUI _scoreText = null;

    public void Reset() {
        _resultUIObject.gameObject.SetActive(false);
    }
    
    public void ShowResultUI(bool isCleared, string timeAgo) {
        _resultUIObject.gameObject.SetActive(true);

        _titleText.text = isCleared ? "CLEAR" : "GAME OVER";
        _timeText.text = timeAgo;
        _scoreText.text = "0";
    }
}
