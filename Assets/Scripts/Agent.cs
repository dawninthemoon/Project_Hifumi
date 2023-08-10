using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

[System.Serializable]
public struct CircleColliderConfig {
    public float radius;
    public Vector2 offset;
    public Color color;
}

public class Agent : MonoBehaviour {
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private CircleCollider _bodyCollider = null;
    [SerializeField] private CircleCollider _attackRange = null;
    [SerializeField] private int _attackDamage = 5;
    public float Radius {
        get { return _bodyCollider.CircleShape.radius; }
    }
    public int Hp { get; set; } = 100;
    private float _movedTime;
    private static readonly float RequireMoveTime = 0.5f;
    private Animator _animatorController;
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

    private void Start() {
        _bodyCollider.OnCollisionEvent.AddListener(EdgeOut);
        _attackRange.OnCollisionEvent.AddListener(
            (CustomCollider self, CustomCollider other) => {
                if (DoingAttack || DoingMove) return;
                Attack(other.transform.parent.GetComponent<Agent>());
            }
        );
    }

    public void Move(Agent target) {
        _movedTime += Time.deltaTime;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector3 targetPosition = target.transform.position;
        targetPosition -= dir * (target.Radius + _attackRange.CircleShape.radius - 0.1f);

        float t = (Time.deltaTime * _moveSpeed) / Vector2.Distance(transform.position, targetPosition);
        Vector3 nextPosition = Vector3.Lerp(transform.position, targetPosition, t);

        _animatorController.SetBool("isMoving", !nextPosition.Equals(transform.position));
        GetComponent<SpriteRenderer>().flipX = (dir.x < 0f);

        transform.position = nextPosition;
    }

    public void Attack(Agent target) {
        _movedTime = 0f;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        float radian = Mathf.Atan2(dir.y, dir.x);

        DoingAttack = true;
        Invoke("DisableAttackTrigger", 0.5f);
        target.Hp -= 10;

        _animatorController.SetTrigger("doAttack");
    }

    private void DisableAttackTrigger() {
        DoingAttack = false;
    }

    private void Update() {
        var hpBar = transform.Find("HpBar");
        var hpBarScale = hpBar.localScale;
        hpBarScale.x = Hp / 100f * 0.7f;
        hpBar.localScale = hpBarScale;

    }

    private void EdgeOut(CustomCollider self, CustomCollider other) {
        Transform selfObj = self.transform.parent;
        Transform otherObj = other.transform.parent;

        float radius = (self as CircleCollider).CircleShape.radius;
        Vector3 dir = (selfObj.transform.position - otherObj.transform.position).normalized;
        selfObj.transform.position += dir * (radius * 0.1f);
        otherObj.transform.position -= dir * (radius * 0.1f);
    }
}
