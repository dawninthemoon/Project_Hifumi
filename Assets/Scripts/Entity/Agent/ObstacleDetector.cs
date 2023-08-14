using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class ObstacleDetector : Detector {
    [SerializeField] private float _detectionRadius = 72f;
    [SerializeField] private ColliderLayerMask _layerMask;
    private CustomCollider[] _colliders;
    
    public override void Detect(AIData aiData) {
        _colliders = CollisionManager.Instance.OverlapCircleAll(transform.position, _detectionRadius, _layerMask);
        aiData.obstacles = _colliders;
    }
}
