using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class EntityBase : MonoBehaviour {
    [SerializeField] private CircleCollider _bodyCollider = null;
    [SerializeField] private CircleCollider _attackRange = null;
    [SerializeField] private AttackConfig _attackConfig;
    [SerializeField] private AttackConfig _skillConfig;
    [SerializeField] private Transform _hpBarTransform = null;
    [SerializeField] private Transform _mpBarTransform = null;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _maxMana = 100;
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
    public int Mana { get; private set; }
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

        var mpBarScale = _mpBarTransform.localScale;
        mpBarScale.x = (float)Mana / _maxMana * 0.7f;
        _mpBarTransform.localScale = mpBarScale;

        Debug.DrawRay(transform.position, _faceDir * 0.5f, Color.cyan);
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
        Mana = Mathf.Min(Mana + 10, _maxMana);

        DoingAttack = true;
        Invoke("DisableAttackTrigger", 0.5f);

        string triggerName = "doAttack";
        AttackConfig config = _attackConfig;
        if (Mana == _maxMana) {
            Mana = 0;
            triggerName = "doSkill";
            config = _skillConfig;
        }

        _animatorController.SetTrigger(triggerName);

        var effects = config.attackEffects;
        config.attackBehaviour.Behaviour(this, _entitiesInAttackRange, effects);
    }

    public void SetMoveAnimationState(bool isMoving) {
        _animatorController.SetBool("isMoving", isMoving);
    }

    public void ReceiveDamage(int damage) {
        Mana = Mathf.Min(Mana + 10, _maxMana);
        Health -= damage;
    }

    private void DisableAttackTrigger() {
        DoingAttack = false;
    }
}
