using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityBase : MonoBehaviour {
    [SerializeField] private float _bodyRadius = 20f;
    private Agent _agent;
    private EntityInfo _entityInfo = null;
    private EntityAnimationControl _animationControl;
    private EntityUIControl _uiControl;
    private EntityStatusDecorator _statusDecorator;
    private int _currentHealth;
    private int _currentMana;
    private float _currentMorale;
    public float Radius {
        get { return _bodyRadius; }
    }
    public string ID {
        get { return _entityInfo.EntityID; }
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
    public float Morale {
        get { return _currentMorale; }
        set {
            _currentMorale = value;
            _uiControl.UpdateMoraleUI(Mathf.FloorToInt(_currentMorale), _statusDecorator.Morale);
        }
    }
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
        
        _agent.Initialize(_statusDecorator);
        _agent.OnMovementInput.AddListener((direction) => {
            _animationControl.SetMoveAnimationState(!direction.Equals(Vector2.zero));
            if (direction.sqrMagnitude > 0f)
                _animationControl.SetFaceDir(direction);
        });

        InitalizeStatus();
    }

    public void SetEntitySelected(bool active) {
        _uiControl.SetMoraleUIActive(active);
    }

    private void InitalizeStatus() {
        _currentHealth = _statusDecorator.Health;
        _currentMana = _statusDecorator.Mana;
        _currentMorale = _statusDecorator.Morale;
    }

    public void SetTarget(ITargetable target) {
        _agent.SetTarget(target);
    }

    private void Attack() {
        Mana = Mathf.Min(Mana + 10, _statusDecorator.Mana);

        //DoingAttack = true;

        AttackConfig config = _entityInfo.EntityAttackConfig;
        /*if (Mana == _maxMana) {
            // Use Skill
            Mana = 0;
            config = _entityInfo.EntitySkillConfig;
        }*/
        _animationControl.PlayAttackAnimation();

        var effects = config.attackEffects;
        List<EntityBase> targets 
            = Physics2D.OverlapCircleAll(transform.position, _statusDecorator.AttackRange + Radius * 2.5f, config.targetLayerMask)
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
}
