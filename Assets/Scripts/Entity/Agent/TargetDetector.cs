using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class TargetDetector : Detector {
    [SerializeField] private float _targetDetectionRange = 100f;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private LayerMask _targetLayerMask;
    private List<Transform> _colliders;
    private Transform _cachedTargetTransform;
    private static readonly string EnemyTagString = "Enemy";

    private void Awake() {
        _cachedTargetTransform = new GameObject().transform;
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
                // 상대의 이전 위치가 존재하지 않을 때, 상대 위치를 기준으로 이동할 수 있는 방향을 탐색
                float sign = gameObject.tag.Equals(EnemyTagString) ? 1f : -1f;
                direction = (aiData.SelectedTarget.Position - transform.position);
                for (int i = 1; i < 32; ++i) {
                    float radian = Mathf.Atan2(direction.y, direction.x) + i * Mathf.PI / 16f;
                    Vector3 dir = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
                    float dist = Vector2.Distance(transform.position, aiData.SelectedTarget.Position);

                    Vector2 raycastPosition = aiData.SelectedTarget.Position + dist * dir;
                    float distToRaycastPosition = Vector2.Distance(transform.position, raycastPosition);
                    Vector2 directionToRaycastPos = (raycastPosition - (Vector2)transform.position).normalized;
                    
                    var hitToRaycastPos = Physics2D.Raycast(transform.position, directionToRaycastPos, distToRaycastPosition, _obstaclesLayerMask);
                    if (hitToRaycastPos.collider != null) continue;

                    var hit2 = Physics2D.Raycast(raycastPosition, -dir, _targetDetectionRange, _obstaclesLayerMask);
                    if (hit2.collider != null && (_targetLayerMask & (1 << hit2.collider.gameObject.layer)) != 0) {
                        _cachedTargetTransform.position = raycastPosition;
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