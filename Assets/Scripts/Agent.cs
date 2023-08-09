using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _radius = 0.5f;
    public float Radius {
        get { return _radius; }
    }

    public void Move(Agent target) {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector3 targetPosition = target.transform.position;
        float offset = 0.2f;
        targetPosition -= dir * (target.Radius * 2f + offset);

        float t = (Time.deltaTime * _moveSpeed) / Vector2.Distance(transform.position, targetPosition);
        transform.position = Vector3.Lerp(transform.position, targetPosition, t);
    }

    private void Update() {
        DrawCircle();
    }

    private void DrawCircle() {
        Vector2 position = (Vector2)transform.position;
        Vector2 cur = position + _radius * new Vector2(Mathf.Cos(0f), Mathf.Sin(0f));
        Vector2 prev = cur;
        
        for (float theta = 0.1f; theta < Mathf.PI * 2f; theta += 0.1f) {
            cur = position + _radius * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            Debug.DrawLine(cur, prev, Color.green);
            prev = cur;
        }
        cur = position + _radius * new Vector2(Mathf.Cos(0f), Mathf.Sin(0f));
        Debug.DrawLine(cur, prev, Color.green);
    }
}
