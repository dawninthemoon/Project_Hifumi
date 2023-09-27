using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SliderableUI : MonoBehaviour {
    [SerializeField] private RectTransform _content;
    [SerializeField] private Button _button;
    [SerializeField] private Vector2 _moveDirection;
    [SerializeField] private bool _isEnabledAtStart;
    [SerializeField] private float _duration = 0.5f;
    private bool _isEnabled;
    private Rect _contentRect;

    private void Awake() {
        _isEnabled = _isEnabledAtStart;
        _contentRect = _content.rect;
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked() {
        Vector2 moveVector;
        moveVector.x = _contentRect.width * _moveDirection.x;
        moveVector.y = _contentRect.height * _moveDirection.y;
        moveVector *= _isEnabled ? 1f : -1f;
        _isEnabled = !_isEnabled;

        transform.DOLocalMove(moveVector, _duration).SetRelative();
    }
}
