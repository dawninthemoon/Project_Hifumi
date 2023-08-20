using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityBase : MonoBehaviour {
    [SerializeField] private string _id = null;
    [SerializeField] private float _bodyRadius = 20f;
    [SerializeField] private AttackConfig _attackConfig;
    [SerializeField] private AttackConfig _skillConfig;
    private Agent _agent;
    private EntityInfo _entityInfo = null;
    private EntityAnimationControl _animationControl;
    private EntityUIControl _uiControl;
    private EntityStatusDecorator _statusDecorator;
    private int _currentHealth;
    private int _currentMana;
    public float Radius {
        get { return _bodyRadius; }
    }
    public string ID {
        get { return _id; }
    }
    public int Health { 
        get { return _currentHealth; }
        set { 
            _currentHealth = value;
            _uiControl.UpdateHealthBar(_currentHealth, _statusDecorator.Health);
        }
    }
    public int Mana { 
        get { return _currentMana; }
        set { 
            _currentMana = value;
            _uiControl.UpdateManaBar(_currentMana, _statusDecorator.Mana);
        }
    }
    public int Stress { get; set; }
    public int AttackDamage { get { return _statusDecorator.AttackDamage; } }

    private void Awake() {
        _agent = GetComponent<Agent>();
        _animationControl = GetComponent<EntityAnimationControl>();
        _uiControl = GetComponent<EntityUIControl>();

        _agent.OnAttackRequested.AddListener(Attack);
    }

    public void Initialize(EntityInfo entityInfo) {
        _entityInfo = entityInfo;
        _statusDecorator = new EntityStatusDecorator(_entityInfo);

        _animationControl.Initialize(_entityInfo.BodySprite, _entityInfo.WeaponSprite, _entityInfo.AnimatorController);
        
        _agent.Initialize(_statusDecorator.MoveSpeed, _statusDecorator.AttackRange);
        _agent.OnMovementInput.AddListener((direction) => {
            _animationControl.SetMoveAnimationState(!direction.Equals(Vector2.zero));
            if (direction.sqrMagnitude > 0f)
                _animationControl.SetFaceDir(direction);
        });

        _currentHealth = _statusDecorator.Health;
        _currentMana = _statusDecorator.Mana;
    }

    public void SetTarget(ITargetable target) {
        _agent.SetTarget(target);
    }

    private void Attack() {
        Mana = Mathf.Min(Mana + 10, _statusDecorator.Mana);

        //DoingAttack = true;

        AttackConfig config = _attackConfig;
        /*if (Mana == _maxMana) {
            // Use Skill
            Mana = 0;
            config = _skillConfig;
        }*/
        _animationControl.PlayAttackAnimation();

        var effects = config.attackEffects;
        List<EntityBase> targets 
            = Physics2D.OverlapCircleAll(transform.position, _agent.AttackDistance + Radius * 2.5f, config.targetLayerMask)
                .Select(x => x.GetComponent<EntityBase>())
                .Where(x => x.Health > 0)
                .OrderBy(x => (x.transform.position - transform.position).sqrMagnitude)
                .ToList();

        if (targets.Count > 0) {
            Vector2 direction = (targets[0].transform.position - transform.position).normalized;
            _animationControl.SetFaceDir(direction);
        }
        
        config.attackBehaviour.Behaviour(this, targets, effects);
    }

    public void ReceiveDamage(int damage) {
        if (Health <= 0) return;

        Mana = Mathf.Min(Mana + 10, _statusDecorator.Mana);
        Health -= damage;
        if (Health <= 0) {
           OnEntityDead();
        }
    }

    private void OnEntityDead() {
        gameObject.SetActive(false);
        /*
        SetTarget(null);
        Destroy(_agent);
        //_agent.enabled = false;
        _animationControl.PlayDeadAnimation();*/
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (this is Truck || other.name.Equals("Truck")) return;
        GameObject otherObj = other.gameObject;
        if (otherObj.layer.Equals(LayerMask.NameToLayer("Ally")) || otherObj.layer.Equals(LayerMask.NameToLayer("Enemy"))) {
            Vector3 direction = (transform.position - other.transform.position).normalized;
            float force = 10f;
            transform.position += direction * force * Time.deltaTime;
        }
    }
}
