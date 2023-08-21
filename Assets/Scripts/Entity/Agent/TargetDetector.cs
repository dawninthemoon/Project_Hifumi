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
    private Vector2 _rightDirection;

    private void Awake() {
        _cachedTargetTransform = new GameObject().transform;

        float radian = Mathf.PI / 16f;
        _rightDirection = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
    }

    public override void Detect(AIData aiData) {
        if (aiData.SelectedTarget != null) {
            Vector2 direction = (aiData.SelectedTarget.Position - transform.position).normalized;
            var hit = Physics2D.Raycast(transform.position, direction, _targetDetectionRange, _obstaclesLayerMask);

            if (hit.collider != null && (_targetLayerMask & (1 << hit.collider.gameObject.layer)) != 0) {
                //Debug.DrawRay(transform.position, direction * _targetDetectionRange, Color.magenta);

                Vector3 targetPosition = aiData.SelectedTarget.Position;
                _cachedTargetTransform.position = targetPosition;

                aiData.CurrentTarget = _cachedTargetTransform;
            }
            else {
                bool isTargetSelected = false;
                foreach (Vector2 scentPosition in aiData.SelectedTarget.GetScentTrail()) {
                    direction = (scentPosition - (Vector2)transform.position).normalized;
                    hit = Physics2D.Raycast(transform.position, direction, _targetDetectionRange, _obstaclesLayerMask);
                    //Debug.DrawRay(transform.position, direction * _targetDetectionRange, Color.cyan);
                    if (hit.collider == null) {
                        _cachedTargetTransform.position = scentPosition;
                        aiData.CurrentTarget = _cachedTargetTransform;
                        isTargetSelected = true;
                        break;
                    }
                }

                if (isTargetSelected) return;
                
                // 상대의 이전 위치가 존재하지 않을 때, 이동할 수 있는 방향을 하나 선택
                for (int i = 1; i < 32; ++i) {
                        Vector3 left = (direction - _rightDirection * i).normalized;
                        Vector3 right = (direction + _rightDirection * i).normalized;

                        var hitLeft = Physics2D.Raycast(transform.position, left, _targetDetectionRange, _obstaclesLayerMask);
                        var hitRight = Physics2D.Raycast(transform.position, right, _targetDetectionRange, _obstaclesLayerMask);

                        if (hitLeft.collider == null) {
                            _cachedTargetTransform.position = transform.position + left * _targetDetectionRange;
                            aiData.CurrentTarget = _cachedTargetTransform;
                            break;
                        }
                        else if (hitRight.collider == null) {
                            _cachedTargetTransform.position = transform.position + right * _targetDetectionRange;
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
}