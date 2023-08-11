using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackBehaviours.Effects;
using CustomPhysics;

public class TraceProjectile : ProjectileBase {
    private List<EntityBase> _cachedEntityList = new List<EntityBase>(1);
    private Transform _target;
    private float _moveSpeed;
    private bool _removeSelf;

    public override void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects) {
        _target = target.transform;
        _moveSpeed = moveSpeed;

        _projectileBody = GetComponent<CustomCollider>();
        _projectileBody.OnCollisionEvent.AddListener((CustomCollider c1, CustomCollider c2) => {
            var other = c2.transform.parent;
            if (!other.Equals(target.transform)) return;

            _cachedEntityList.Clear();
            _cachedEntityList.Add(target);

            foreach (IAttackEffect effect in effects) {
                effect.ApplyEffect(caster, _cachedEntityList);
            }

            _removeSelf = true;
        });
    }

    protected override void Update() {
        if (_target == null) {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = _target.position;
        float t = (Time.deltaTime * _moveSpeed) / Vector2.Distance(transform.position, targetPosition);

        Vector3 nextPosition = Vector3.Lerp(transform.position, targetPosition, t);
        transform.position = nextPosition;

        Vector2 dir = targetPosition - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void LateUpdate() {
        if (_removeSelf)
            Destroy(gameObject);
    }
}
