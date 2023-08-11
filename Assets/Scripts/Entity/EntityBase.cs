using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class EntityBase : MonoBehaviour {
    [SerializeField] private CircleCollider _bodyCollider = null;
    [SerializeField] private CircleCollider _attackRange = null;
    [SerializeField] private Transform _hpBarTransform = null;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _attackDamage = 5;
    private Agent _agent;
    private Animator _animatorController;
    private SpriteRenderer _spriteRenderer;
    public float Radius {
        get { return _bodyCollider.CircleShape.radius; }
    }
    public bool DoingAttack { get; private set; }
    public int Health { get; private set; }

    private void Awake() {
        _agent = new Agent(_bodyCollider, _moveSpeed);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animatorController = GetComponent<Animator>();
    }

    private void Start() {
        Health = _maxHealth;

        _attackRange.OnCollisionEvent.AddListener(
            (CustomCollider self, CustomCollider other) => {
                if (DoingAttack || _agent.DoingMove) return;
                Attack(other.transform.parent.GetComponent<EntityBase>());
            }
        );
    }

    private void Update() {
        var hpBarScale = _hpBarTransform.localScale;
        hpBarScale.x = (float)Health / _maxHealth * 0.7f;
        _hpBarTransform.localScale = hpBarScale;
    }

    public void Move(EntityBase target) {
        var movedEntityInfo = _agent.Move(transform, target, _attackRange.CircleShape.radius);
        SetMoveAnimationState(!movedEntityInfo.Item2);
        _spriteRenderer.flipX = (movedEntityInfo.Item1 < 0f);
    }

    public void Attack(EntityBase target) {
        _agent.MovedTime = 0f;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        float radian = Mathf.Atan2(dir.y, dir.x);

        DoingAttack = true;
        Invoke("DisableAttackTrigger", 0.5f);

        target.Health -= _attackDamage;

        _animatorController.SetTrigger("doAttack");
    }

    public void SetMoveAnimationState(bool isMoving) {
        _animatorController.SetBool("isMoving", isMoving);
    }

    public void ReceiveDamage(int damage) {
        Health -= damage;
    }

    private void DisableAttackTrigger() {
        DoingAttack = false;
    }
}
