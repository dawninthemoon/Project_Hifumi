using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObstacleDetector : Detector {
    [SerializeField] private float _detectionRadius = 72f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Collider2D _colliderSelf = null;
    
    public override void Detect(AIData aiData) {
        var colliders = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _layerMask).ToList();
        if (colliders.Contains(_colliderSelf))
            colliders.Remove(_colliderSelf);
        aiData.Obstacles = colliders;
    }
}
