using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CircleColliderConfig {
    public float radius;
    public Vector2 offset;
    public Color color;
}

public class Agent : MonoBehaviour {
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private CircleColliderConfig _collisionRange;
    [SerializeField] private CircleColliderConfig _attackRange;
    [SerializeField] private float _attackDistance = 0.2f;
    [SerializeField] private Vector2 _attackRectSize = Vector2.one;
    public int Hp { get; set; } = 100;
    public float _movedTime;
    private static readonly float RequireMoveTime = 0.5f;
    private Animator _animatorController;
    public float Radius {
        get { return _collisionRange.radius; }
    }
    public float AttackRadius {
        get { return _attackRange.radius; }
    }
    public bool DoingAttack { get; private set; }
    public bool DoingMove { 
        get {
            return _movedTime < RequireMoveTime;
        }
    }

    private void Awake() {
        _animatorController = GetComponent<Animator>();
        _movedTime = RequireMoveTime;
    }

    public void Move(Agent target) {
        _movedTime += Time.deltaTime;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector3 targetPosition = target.transform.position;
        targetPosition -= dir * (target.Radius + _attackDistance - 0.1f);

        float t = (Time.deltaTime * _moveSpeed) / Vector2.Distance(transform.position, targetPosition);
        Vector3 nextPosition = Vector3.Lerp(transform.position, targetPosition, t);

        Debug.DrawRay(transform.position, dir * Vector2.Distance(transform.position, targetPosition), _attackRange.color);
        
        _animatorController.SetBool("isMoving", !nextPosition.Equals(transform.position));
        GetComponent<SpriteRenderer>().flipX = (dir.x < 0f);

        transform.position = nextPosition;
    }

    public void Attack(Agent target) {
        _movedTime = 0f;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector2 attackPosition = transform.position + dir * _attackDistance;

        float radian = Mathf.Atan2(dir.y, dir.x);
        CreateAttackTest(attackPosition, radian + Mathf.PI * 0.5f);

        target.Hp -= 10;

        _animatorController.SetTrigger("doAttack");
    }

    private void CreateAttackTest(Vector2 pos, float radian) {
        DoingAttack = true;
        Invoke("DisableAttackTrigger", 0.5f);
        GameObject obj = new GameObject();
        obj.AddComponent<AttackRangeTest>().Initialize(pos, _attackRectSize, radian, 1f);
    }

    private void DisableAttackTrigger() {
        DoingAttack = false;
    }

    private void Update() {
        var hpBar = transform.GetChild(0);
        var hpBarScale = hpBar.localScale;
        hpBarScale.x = Hp / 100f * 0.7f;
        hpBar.localScale = hpBarScale;

    }

    private void OnDrawGizmos() {
        DrawCircleGizmos(_collisionRange.offset, _collisionRange.radius, _collisionRange.color);
        DrawCircleGizmos(_attackRange.offset, _attackRange.radius, _attackRange.color);

        void DrawCircleGizmos(Vector2 offset, float radius, Color color) {
            Color prevColor = Gizmos.color;
            Gizmos.color = color;

            Vector2 position = (Vector2)transform.position + offset;
            Vector2 cur = position + radius * new Vector2(Mathf.Cos(0f), Mathf.Sin(0f));
            Vector2 prev = cur;
            
            for (float theta = 0.1f; theta < Mathf.PI * 2f; theta += 0.1f) {
                cur = position + radius * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
                Gizmos.DrawLine(cur, prev);
                prev = cur;
            }
            cur = position + radius * new Vector2(Mathf.Cos(0f), Mathf.Sin(0f));
            Gizmos.DrawLine(cur, prev);

            Gizmos.color = prevColor;
        }
    }
}
