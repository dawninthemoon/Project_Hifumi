using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RieslingUtils;

public class HitEffect : MonoBehaviour {
    [SerializeField] private SpriteRenderer _bodyRenderer = null;
    [SerializeField] private float _knockbackDuration = 0.5f;
    [SerializeField] private float _friction = 1f;
    private Vector2 _prevNormal;
    private Vector2 _knockbackDir;
    private float _knockbackForce;
    private float _timeAgo;
    private bool _applyKnockback;
    private Sequence _flashSequence;
    private EntityBase _entityBase;
    private static readonly string FlashAmountKey = "_FlashAmount";

    private void Start() {
        _entityBase = GetComponent<EntityBase>();
    }

    private void OnEnable() {
        _applyKnockback = false;
    }

    private void Update() {
        if (_applyKnockback && (_knockbackDuration > _timeAgo)) {
            //Vector2 normal = CombatMap.GetBoarderNormal(transform.position, _radius);
            //ApplyReflect(normal);

            Vector3 knockbackAmount = _knockbackDir * Mathf.Max(0f, _knockbackForce - _friction * _timeAgo);
            transform.position += knockbackAmount * Time.deltaTime;
            _timeAgo += Time.deltaTime;
        }
    }

    public void ApplyKnockback(Vector2 direction, float force, int damage, float freezeDuration, DebuffConfig debuff) {
        StartCoroutine(ApplyHitEffect(direction, force, damage, freezeDuration, debuff));
    }

    private IEnumerator ApplyHitEffect(Vector2 direction, float force, int damage, float freezeDuration, DebuffConfig debuff) {
        _bodyRenderer.material.SetFloat(FlashAmountKey, 1f);

        yield return YieldInstructionCache.WaitForSeconds(freezeDuration);

        _bodyRenderer.material.SetFloat(FlashAmountKey, 0f);

        _knockbackDir = direction;
        _knockbackForce = force;
        _timeAgo = 0f;
        _applyKnockback = true;

        _entityBase.ReceiveDamage(damage);
        _entityBase.BuffControl.StartAddDebuff(debuff);
    }

    private void ApplyReflect(Vector2 normal) {
        if (!normal.Equals(Vector2.zero) && (!_prevNormal.Equals(normal))) {
            _prevNormal = normal;
            _knockbackDir = GetReflectVector(normal);
        }
    }

    private Vector2 GetReflectVector(Vector2 n) {
        Vector2 reflectVector = _knockbackDir + 2 * n * Vector2.Dot(-_knockbackDir, n);
        return reflectVector;
    }
}
