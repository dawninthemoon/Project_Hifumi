using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class TestCode : MonoBehaviour {
    void Update() {
        Vector2 point = RieslingUtils.MouseUtils.GetMouseWorldPosition();
        Vector2 dir = (point - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(point, transform.position);

        CollisionManager.Instance.Raycast(transform.position, dir, distance, out RaycastInfo info);
        Debug.DrawRay(transform.position, dir * distance, Color.green);
        Debug.Log(info.collider != null);
    }
}
