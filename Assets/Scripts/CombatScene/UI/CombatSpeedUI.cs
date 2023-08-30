using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatSpeedUI : MonoBehaviour {
    [SerializeField] private Button _speedControlButton = null;
    [SerializeField] private Sprite[] _buttonSprites = null;
    private int _currentButtonIndex = 0;
    private int _numOfButtonType;

    private void Awake() {
        _numOfButtonType = _buttonSprites.Length;
        _currentButtonIndex = Mathf.FloorToInt(GameConfigHandler.GameSpeed - 1f) % _numOfButtonType;
        _speedControlButton.image.sprite = _buttonSprites[_currentButtonIndex];

        _speedControlButton.onClick.AddListener(() => {
            _currentButtonIndex = (_currentButtonIndex + 1) % _numOfButtonType;

            Sprite currSprite = _buttonSprites[_currentButtonIndex];
            _speedControlButton.image.sprite = currSprite;
            GameConfigHandler.GameSpeed = (_currentButtonIndex + 1);
        });
    }
}
