using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class ObstacleDetector : Detector {
    [SerializeField] private float _detectionRadius = 72f;
    [SerializeField] private LayerMask _layerMask;
    private Collider2D[] _colliders;
    
    public override void Detect(AIData aiData) {
        _colliders = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _layerMask);
        aiData.Obstacles = _colliders;
    }
}
