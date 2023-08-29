using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitEffect : MonoBehaviour {
    [SerializeField] private SpriteRenderer _bodyRenderer = null;
    [SerializeField] private float _knockbackDuration = 0.5f;
    [SerializeField] private float _freezeDuration = 0.15f;
    [SerializeField] private float _flashDuration = 0.15f;
    [SerializeField] private float _friction = 1f;
    private Vector2 _prevNormal;
    private Vector2 _knockbackDir;
    private float _knockbackForce;
    private float _timeAgo;
    private bool _applyKnockback;
    private Sequence _flashSequence;
    private Sequence _timeFreezeSequence;
    private float _radius;

    private void Awake() {
        _radius = GetComponent<EntityBase>().Radius;
    }

    private void OnEnable() {
        _applyKnockback = false;
    }

    private void Update() {
        if (_applyKnockback && (_knockbackDuration > _timeAgo)) {
            Vector2 normal = CheckBoarder();
            if (!normal.Equals(Vector2.zero) && (!_prevNormal.Equals(normal))) {
                _prevNormal = normal;
                _knockbackDir = GetReflectVector(normal);
            }

            Vector3 knockbackAmount = _knockbackDir * Mathf.Max(0f, _knockbackForce - _friction * _timeAgo);
            transform.position += knockbackAmount * Time.deltaTime;
            _timeAgo += Time.deltaTime;
        }
    }

    public void ApplyKnockback(Vector2 direction, float force) {
        if (_applyKnockback) {
            return;
        }

        _knockbackDir = direction;
        _knockbackForce = force;
        _timeAgo = 0f;

        StartFlash();
        _applyKnockback = true;
        StartFreezeTime();
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

    private Vector2 CheckBoarder() {
        Vector2 maxSize = CombatSceneHandler.StageMaxSize;
        Vector2 minSize = CombatSceneHandler.StageMinSize;
        Vector2 pos = transform.position;
        Vector2 normal = Vector2.zero;

        if (pos.y > maxSize.y - _radius) {
            normal = Vector2.down;
        }
        else if (pos.x < minSize.x + _radius) {
            normal = Vector2.right;
        }
        else if (pos.y < minSize.y + _radius) {
            normal = Vector2.up;
        }
        else if (pos.x > maxSize.x - _radius) {
            normal = Vector2.left;
        }
        return normal;
    }

    private Vector2 GetReflectVector(Vector2 n) {
        Vector2 reflectVector = _knockbackDir + 2 * n * Vector2.Dot(-_knockbackDir, n);
        return reflectVector;
    }
}
