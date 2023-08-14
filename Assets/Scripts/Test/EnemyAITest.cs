using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAITest : MonoBehaviour {
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _moveSpeed = 30f;
    [SerializeField] private float _rayDistance = 10f;
    private Vector2 _direction;

    private void ChaseTarget() {
        _direction = (_target.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, _rayDistance);
        Debug.DrawRay(transform.position, _direction * _rayDistance, Color.green);
        if (hit.collider != null) {
            _direction = Vector2.zero;
            foreach (Vector2 pos in ScentTest.ScentTrail) {
                Vector2 tempDir = (pos - (Vector2)transform.position).normalized;
                RaycastHit2D hit2 = Physics2D.Raycast(transform.position, tempDir, _rayDistance);
                Debug.DrawRay(transform.position, tempDir * _rayDistance, Color.blue);
                if (hit2.collider == null) {
                    _direction = tempDir;
                    break;
                }
            }
        }
    }

    private void Update() {
        ChaseTarget();
        Vector3 moveVector = _direction * _moveSpeed * Time.deltaTime;
        transform.position += moveVector;
    }
}
