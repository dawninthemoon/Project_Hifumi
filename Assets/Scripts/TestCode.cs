using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class TestCode : MonoBehaviour {
    private CircleCollider _collider;
    void Awake() {
        _collider = GetComponent<CircleCollider>();
    }

    void Update() {
        CustomCollider[] colliders = CollisionManager.Instance.OverlapCircleAll(_collider.CircleShape.center, _collider.CircleShape.radius, ColliderLayerMask.None);
        
        foreach (CustomCollider collider in colliders) {
            RectCollider rect = collider as RectCollider;
            if (rect) {
                Vector2 pos = rect.GetClosestPoint(transform.position);
                Debug.DrawLine(transform.position, pos, Color.green);
            }
        }
    }
}
