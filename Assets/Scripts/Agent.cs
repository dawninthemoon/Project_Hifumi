using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private float _attackRadius = 0.7f;
    [SerializeField] private float _attackDistance = 0.2f;
    [SerializeField] private Vector2 _attackRectSize = Vector2.one;
    public float Radius {
        get { return _radius; }
    }
    public float AttackRadius {
        get { return _attackRadius; }
    }

    public void Move(Agent target) {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector3 targetPosition = target.transform.position;
        float offset = 0.1f;
        targetPosition -= dir * (target.Radius * 2f + offset);

        float t = (Time.deltaTime * _moveSpeed) / Vector2.Distance(transform.position, targetPosition);
        transform.position = Vector3.Lerp(transform.position, targetPosition, t);
    }

    public void Attack(Agent target) {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector2 attackPosition = transform.position + dir * _attackDistance;

        float radian = Mathf.Atan2(dir.y, dir.x);
        DrawRect(attackPosition, _attackRectSize, radian + Mathf.PI * 0.5f, Color.blue);
    }

    private void Update() {
        DrawCircle(_radius, Color.green);
        DrawCircle(_attackRadius, Color.blue);
    }

    private void DrawRect(Vector2 center, Vector2 size, float radian, Color color) {
        Vector2 dir1 = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
        dir1 = dir1 * _attackRectSize.x;

        Vector2 dir2 = new Vector2(Mathf.Cos(radian + Mathf.PI * 0.5f), Mathf.Sin(radian + Mathf.PI * 0.5f)).normalized;
        dir2 = dir2 * _attackRectSize.y;

        Debug.DrawLine(center + dir1 + dir2, center - dir1 + dir2, color);
        Debug.DrawLine(center - dir1 + dir2, center - dir1 - dir2, color);
        Debug.DrawLine(center - dir1 - dir2, center + dir1 - dir2, color);
        Debug.DrawLine(center + dir1 - dir2, center + dir1 + dir2, color);
    }

    private void DrawCircle(float radius, Color color) {
        Vector2 position = (Vector2)transform.position;
        Vector2 cur = position + radius * new Vector2(Mathf.Cos(0f), Mathf.Sin(0f));
        Vector2 prev = cur;
        
        for (float theta = 0.1f; theta < Mathf.PI * 2f; theta += 0.1f) {
            cur = position + radius * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            Debug.DrawLine(cur, prev, color);
            prev = cur;
        }
        cur = position + radius * new Vector2(Mathf.Cos(0f), Mathf.Sin(0f));
        Debug.DrawLine(cur, prev, color);
    }
}
