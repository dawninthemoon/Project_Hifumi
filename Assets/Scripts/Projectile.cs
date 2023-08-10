using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private float _moveSpeed = 1f;
    private Transform _target;
    private int _damage;

    public void Initialize(Transform target, int damage) {
        _target = target;
        _damage = damage;
    }

    private void Update() {
        Vector3 dir = (_target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        transform.position += dir * _moveSpeed * Time.deltaTime;
    }
}
