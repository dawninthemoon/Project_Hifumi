using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityBase : MonoBehaviour {
    [SerializeField] private string _id = null;
    [SerializeField] private float _bodyRadius = 20f;
    [SerializeField] private AttackConfig _attackConfig;
    [SerializeField] private AttackConfig _skillConfig;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _maxMana = 100;
    [SerializeField] private int _attackDamage = 5;
    private Agent _agent;
    private EntityAnimationControl _animationControl;
    private EntityUIControl _uiControl;
    private int _health;
    private int _mana;
    public float Radius {
        get { return _bodyRadius; }
    }
    public int MaxMana {
        get { return _maxMana; }
    }
    public string ID {
        get { return _id; }
    }
    public int Health { 
        get { return _health; }
        set { 
            _health = value;
            _uiControl.UpdateHealthBar(_health, _maxHealth);
        }
    }
    public int Mana { 
        get { return _mana; }
        set { 
            _mana = value;
            _uiControl.UpdateManaBar(_mana, _maxMana);
        }
    }
    public int Stress { get; set; }
    public int AttackDamage { get { return _attackDamage; } }

    public void Initialize() {
        _agent = GetComponent<Agent>();
        _animationControl = GetComponent<EntityAnimationControl>();
        _uiControl = GetComponent<EntityUIControl>();

        _agent.OnMovementInput.AddListener((direction) => {
            _animationControl.SetMoveAnimationState(!direction.Equals(Vector2.zero));
            if (direction.sqrMagnitude > 0f)
                _animationControl.SetFaceDir(direction);
        });
        _agent.OnAttackRequested.AddListener(Attack);

        InitalizeStatus();
    }

    private void InitalizeStatus() {
        Health = _maxHealth;
        Mana = 0;
    }

    public void SetTarget(ITargetable target) {
        _agent.SetTarget(target);
    }

    private void Attack() {
        Mana = Mathf.Min(Mana + 10, _maxMana);

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

        Mana = Mathf.Min(Mana + 10, _maxMana);
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
