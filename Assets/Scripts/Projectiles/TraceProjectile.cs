using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackBehaviours.Effects;
using RieslingUtils;

public class TraceProjectile : ProjectileBase {
    private EntityBase _caster;
    private IAttackEffect[] _effects;
    private List<EntityBase> _cachedEntityList = new List<EntityBase>(1);
    private Transform _target;
    private float _moveSpeed;
    private bool _removeSelf;

    public override void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects) {
        _caster = caster;
        _target = target.transform;
        _moveSpeed = moveSpeed;
        _effects = effects;
    }

    public override void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects, float angle) {
        Initialize(caster, target, moveSpeed, effects);
    }

    protected override void Update() {
        if (_target == null || !_target.gameObject.activeSelf) {
            ProjectileSpawner.Instance.RemoveProjectile(this);
            return;
        }

        Vector3 targetPosition = _target.position;
        float t = (Time.deltaTime * _moveSpeed) / Vector2.Distance(transform.position, targetPosition);

        Vector3 nextPosition = Vector3.Lerp(transform.position, targetPosition, t);
        transform.position = nextPosition;

        Vector2 dir = targetPosition - transform.position;
        float angle = ExVector.GetDegree(transform.position, targetPosition);
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.transform.Equals(_target)) return;

        _cachedEntityList.Clear();
        _cachedEntityList.Add(other.GetComponent<EntityBase>());

        foreach (IAttackEffect effect in _effects) {
            effect.ApplyEffect(_caster, _cachedEntityList);
        }

        ProjectileSpawner.Instance.RemoveProjectile(this);
    }
}
