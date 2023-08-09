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
    public bool DoingAttack { get; private set; }

    public void Move(Agent target) {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector3 targetPosition = target.transform.position;
        targetPosition -= dir * (target.Radius + _attackDistance);

        float t = (Time.deltaTime * _moveSpeed) / Vector2.Distance(transform.position, targetPosition);
        transform.position = Vector3.Lerp(transform.position, targetPosition, t);
    }

    public void Attack(Agent target) {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector2 attackPosition = transform.position + dir * _attackDistance;

        float radian = Mathf.Atan2(dir.y, dir.x);
        CreateAttackTest(attackPosition, radian + Mathf.PI * 0.5f);
    }

    private void CreateAttackTest(Vector2 pos, float radian) {
        DoingAttack = true;
        Invoke("DisableAttackTrigger", 1f);
        GameObject obj = new GameObject();
        obj.AddComponent<AttackRangeTest>().Initialize(pos, _attackRectSize, radian, 1f);
    }

    private void DisableAttackTrigger() {
        DoingAttack = false;
    }

    private void Update() {
        DrawCircle(_radius, Color.green);
        DrawCircle(_attackRadius, Color.blue);
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
