using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class TargetDetector : Detector {
    [SerializeField] private float _targetDetectionRange = 100f;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] private bool _showGizmos = true;
    private List<Transform> _colliders;
    private Transform _cachedTargetTransform;

    private void Awake() {
        _cachedTargetTransform = new GameObject().transform;
    }

    public override void Detect(AIData aiData) {
        if (aiData.SelectedTarget != null) {
            Vector2 direction = (aiData.SelectedTarget.transform.position - transform.position).normalized;
            var hit = Physics2D.Raycast(transform.position, direction, _targetDetectionRange, _obstaclesLayerMask);

            if (hit.collider != null && (_targetLayerMask & (1 << hit.collider.gameObject.layer)) != 0) {
                Debug.DrawRay(transform.position, direction * _targetDetectionRange, Color.magenta);
                _cachedTargetTransform.position = aiData.SelectedTarget.transform.position;
                aiData.CurrentTarget = _cachedTargetTransform;
            }
            else {
                foreach (Vector2 scentPosition in aiData.SelectedTarget.Scent.ScentTrail) {
                    direction = (scentPosition - (Vector2)transform.position).normalized;
                    hit = Physics2D.Raycast(transform.position, direction, _targetDetectionRange, _obstaclesLayerMask);
                    Debug.DrawRay(transform.position, direction * _targetDetectionRange, Color.cyan);
                    if (hit.collider == null) {
                        _cachedTargetTransform.position = scentPosition;
                        aiData.CurrentTarget = _cachedTargetTransform;
                        break;
                    }
                }
            }
        }
        else {
            aiData.CurrentTarget = null;
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