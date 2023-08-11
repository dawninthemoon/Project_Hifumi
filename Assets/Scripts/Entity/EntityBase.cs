using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class EntityBase : MonoBehaviour {
    [SerializeField] private CircleCollider _bodyCollider = null;
    [SerializeField] private CircleCollider _attackRange = null;
    [SerializeField] private AttackConfig _attackConfig;
    [SerializeField] private Transform _hpBarTransform = null;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _attackDamage = 5;
    private Agent _agent;
    private Animator _animatorController;
    private SpriteRenderer _spriteRenderer;
    private List<EntityBase> _entitiesInAttackRange;
    private Vector2 _faceDir;
    public float Radius {
        get { return _bodyCollider.CircleShape.radius; }
    }
    public bool DoingAttack { get; private set; }
    public int Health { get; private set; }
    public int AttackDamage { get { return _attackDamage; } }

    private void Awake() {
        _agent = new Agent(_bodyCollider, _moveSpeed);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animatorController = GetComponent<Animator>();
        _entitiesInAttackRange = new List<EntityBase>();
    }

    private void Start() {
        Health = _maxHealth;

        _attackRange.OnCollisionEvent.AddListener(
            (CustomCollider self, CustomCollider other) => {
                if (DoingAttack || _agent.DoingMove) return;
                _entitiesInAttackRange.Add(other.transform.parent.GetComponent<EntityBase>());
            }
        );
    }

    private void Update() {
        var hpBarScale = _hpBarTransform.localScale;
        hpBarScale.x = (float)Health / _maxHealth * 0.7f;
        _hpBarTransform.localScale = hpBarScale;

        Debug.DrawRay(transform.position, _faceDir * 1f, Color.cyan);
    }

    private void LateUpdate() {
        Attack();
        _entitiesInAttackRange.Clear();
    }

    public void Move(EntityBase target) {
        var movedEntityInfo = _agent.Move(transform, target, _attackRange.CircleShape.radius);
        _faceDir = movedEntityInfo.Item1;
        SetMoveAnimationState(!movedEntityInfo.Item2);
        _spriteRenderer.flipX = (_faceDir.x < 0f);
    }

    public void Attack() {
        if (_entitiesInAttackRange.Count == 0) return;
        _agent.MovedTime = 0f;

        DoingAttack = true;
        Invoke("DisableAttackTrigger", 0.5f);

        _animatorController.SetTrigger("doAttack");

        var attackEffects = _attackConfig.attackEffects;
        _attackConfig.attackBehaviour.Behaviour(this, _entitiesInAttackRange, attackEffects);
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
