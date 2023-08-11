using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackBehaviours.Effects;

public class SimpleProjectile : ProjectileBase {
    private Vector3 _direction;
    private float _moveSpeed;
    public override void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects) {
        _direction = (target.transform.position - transform.position).normalized;
        _moveSpeed = moveSpeed;
    }

    protected override void Update() {
        Vector3 moveVector = _direction * _moveSpeed * Time.deltaTime;
        transform.position += moveVector;
    }
}
