using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class TargetDetector : Detector {
    [SerializeField] private Transform _targetTransform = null;
    [SerializeField] private float _targetDetectionRange = 100f;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private bool _showGizmos = true;
    private List<Transform> _colliders;
    private Transform _cachedTargetTransform;

    private void Awake() {
        _cachedTargetTransform = new GameObject().transform;
    }

    public override void Detect(AIData aiData) {
        if (_targetTransform != null) {
            Vector2 direction = (_targetTransform.transform.position - transform.position).normalized;
            var hit = Physics2D.Raycast(transform.position, direction, _targetDetectionRange, _obstaclesLayerMask);

            if (hit.collider != null && (_playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0) {
                Debug.DrawRay(transform.position, direction * _targetDetectionRange, Color.magenta);
                _cachedTargetTransform.position = _targetTransform.position;
                aiData.currentTarget = _cachedTargetTransform;
            }
            else {
                foreach (Vector2 scentPosition in ScentTest.ScentTrail) {
                    direction = (scentPosition - (Vector2)transform.position).normalized;
                    hit = Physics2D.Raycast(transform.position, direction, _targetDetectionRange, _obstaclesLayerMask);
                    Debug.DrawRay(transform.position, direction * _targetDetectionRange, Color.cyan);
                    if (hit.collider == null) {
                        _cachedTargetTransform.position = scentPosition;
                        aiData.currentTarget = _cachedTargetTransform;
                        break;
                    }
                }
            }
        }
        else {
            aiData.currentTarget = null;
        }
    }

    private void OnGizmoSelected() {
        if (!_showGizmos) return;

        Gizmos.DrawWireSphere(transform.position, _targetDetectionRange);

        if (_colliders == null) return;
        Gizmos.color = Color.magenta;
        foreach (var item in _colliders) {
            Gizmos.DrawSphere(item.position, 20f);
        }
    }
}