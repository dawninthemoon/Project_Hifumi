using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitEffect : MonoBehaviour {
    [SerializeField] private SpriteRenderer _bodyRenderer = null;
    [SerializeField] private float _knockbackDuration = 0.5f;
    [SerializeField] private float _freezeDuration = 0.15f;
    [SerializeField] private float _flashDuration = 0.15f;
    private Vector2 _knockbackDir;
    private float _knockbackForce;
    private float _timeAgo;
    private bool _applyKnockback;
    private Sequence _flashSequence;
    private Sequence _timeFreezeSequence;
    private void Update() {
        if (_applyKnockback && (_knockbackDuration > _timeAgo)) {
            Vector3 knockbackAmount = _knockbackDir * _knockbackForce;
            transform.position += knockbackAmount * Time.deltaTime;
            _timeAgo += Time.deltaTime;
        }
    }

    public void ApplyKnockback(Vector2 direction, float force, bool freezeTime = true) {
        _knockbackDir = direction;
        _knockbackForce = force;
        _timeAgo = 0f;

        StartFlash();
        _applyKnockback = true;
        if (freezeTime) {
            StartFreezeTime();
        }
    }

    private void StartFlash() {
        if (_flashSequence == null) {
            _flashSequence = DOTween.Sequence();
            _flashSequence
                .SetAutoKill(false)
                .AppendCallback(() => { _bodyRenderer.material.SetFloat("_FlashAmount", 1f); })
                .AppendInterval(_flashDuration)
                .AppendCallback(() => { _bodyRenderer.material.SetFloat("_FlashAmount", 0f); });
        }
        else {
            _flashSequence.Restart();
        }
    }

    private void StartFreezeTime() {
        GameConfigHandler.GameSpeed = 0f;
        if (_timeFreezeSequence == null) {
            _timeFreezeSequence = DOTween.Sequence().SetUpdate(true);
            _timeFreezeSequence
                .SetAutoKill(false)
                .AppendCallback(() => { GameConfigHandler.GameSpeed = 0f; })
                .AppendInterval(_freezeDuration)
                .AppendCallback(() => { GameConfigHandler.GameSpeed = 1f; });
        }
        else {
            _timeFreezeSequence.Restart();
        }
    }
}
