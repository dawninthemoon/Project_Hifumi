using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class TargetDetector : Detector {
    [SerializeField] private float _targetDetectionRange = 100f;
    [SerializeField] private ColliderLayerMask _obstaclesLayerMask;
    [SerializeField] private ColliderLayerMask _playerLayerMask;
    [SerializeField] private bool _showGizmos = true;
    private List<Transform> _colliders;
    public override void Detect(AIData aiData) {
        CustomCollider playerCollider = CollisionManager.Instance.OverlapCircle(transform.position, _targetDetectionRange, _playerLayerMask);

        if (playerCollider != null) {
            Vector2 direction = (playerCollider.transform.position - transform.position).normalized;
            /*CollisionManager.Instance.Raycast(transform.position, direction, _targetDetectionRange, _obstaclesLayerMask);
            RaycastHit2D hit
                = Physics2D.Raycast(transform.position, direction, _targetDetectionRange, _obstaclesLayerMask);

            if (hit.collider != null && (_playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0) {*/
                Debug.DrawRay(transform.position, direction * _targetDetectionRange, Color.magenta);
                _colliders = new List<Transform>() { playerCollider.transform };
            /*}
            else {
                _colliders = null;
            }*/
        }
        else {
            _colliders = null;
        }
        aiData.targets = _colliders;
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