using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceProjectile : ProjectileBase {
    private Transform _target;
    private float _moveSpeed;
    public override void Initialize(Transform target, float moveSpeed) {
        _target = target;
        _moveSpeed = moveSpeed;
    }

    protected override void Update() {
        Vector3 targetPosition = _target.position;
        float t = (Time.deltaTime * _moveSpeed) / Vector2.Distance(transform.position, targetPosition);

        Vector3 nextPosition = Vector3.Lerp(transform.position, targetPosition, t);
        transform.position = nextPosition;
    }
}
