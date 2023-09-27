using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityBase : MonoBehaviour, IObserver {
    [SerializeField] private Transform _bulletPosition = null;
    private Agent _agent;
    private EntityAnimationControl _animationControl;
    private EntityUIControl _uiControl;
    private EntityDecorator _entityDecorator;
    private int _currentHealth;
    private int _currentMana;
    private float _currentMorale;
    public EntityInfo Info { get { return _entityDecorator.Info; } }
    public EntityBuff BuffControl {
        get;
        private set;
    }
    public float Radius {
        get {
            if (_entityDecorator == null) {
                return 20f;
            }
            return Info.BodyRadius;
        }
    }
    public string ID {
        get { return Info.EntityID; }
    }
    public int Health { 
        get { return _currentHealth; }
        set { 
            _currentHealth = value;
            _uiControl.UpdateHealthBar(_currentHealth, _entityDecorator.Health);
        }
    }
    public int Mana { 
        get { return _currentMana; }
        set { 
            _currentMana = value;
            _uiControl.UpdateManaBar(_currentMana, _entityDecorator.Mana);
        }
    }
    public float Morale {
        get { return _currentMorale; }
        set {
            _currentMorale = value;
            _uiControl.UpdateMoraleUI(Mathf.FloorToInt(_currentMorale), _entityDecorator.Morale);
        }
    }
    public SynergyType Synergy1 {
        get { return Info.Synergy1; }
    }
    public SynergyType Synergy2 {
        get { return Info.Synergy2; }
    }
    public SynergyType ExtraSynergy {
        get { return _entityDecorator.ExtraSynergy; }
    }

    public Vector2 HandDirection { get; private set; }
    public int AttackDamage { get { return _entityDecorator.AttackDamage; } }
    public Vector3 BulletPosition { get { return _bulletPosition.position; } }
    public bool IsUnloadCompleted { get; set; }
    private bool _canBehaviour = true;

    private void Awake() {
        _agent = GetComponent<Agent>();
        _animationControl = GetComponent<EntityAnimationControl>();
        _uiControl = GetComponent<EntityUIControl>();

        BuffControl = new EntityBuff(this, _entityDecorator);

        _agent.OnMovementInput.AddListener(Move);
        _agent.OnAttackRequested.AddListener(Attack);
    }

    public void Initialize(EntityDecorator entityDecorator) {
        IsUnloadCompleted = false;
        _entityDecorator = entityDecorator;
        _bulletPosition.localPosition = Info.BulletOffset;

        _animationControl.Initialize(Info.BodySprite, Info.WeaponSprite, Info.AnimatorController);
        
        _agent.Initialize(_entityDecorator, Radius);

        InitalizeStatus();
    }

    public void Notify(ObserverSubject subject) {
        PlayerData data = subject as PlayerData;
        if (data == null) {
            return;
        }
    }

    private void Update() {
        if (BuffControl != null) {
            _canBehaviour = !BuffControl.IsDebuffExists("stun");
        }
    }

    public void SetEntitySelected(bool active) {
        _uiControl.SetMoraleUIActive(active);
    }

    private void InitalizeStatus() {
        _currentHealth = _entityDecorator.Health;
        _currentMana = 0;
        _currentMorale = _entityDecorator.Morale;
    }

    public void SetTarget(ITargetable target) {
        _agent?.SetTarget(target);
    }

    private void Move(Vector2 direction) {
        if (!_canBehaviour) {
            return;
        }
        _animationControl.SetMoveAnimationState(!direction.Equals(Vector2.zero));
        if (direction.sqrMagnitude > 0f) {
            Vector3 nextPosition = transform.position + (Vector3)direction * Time.deltaTime * _entityDecorator.MoveSpeed;
            transform.position = nextPosition;

            Vector3 faceDir = direction;
            if (_agent.SelectedTarget != null) {
                faceDir = (_agent.SelectedTarget.Position - transform.position);
            }
            _animationControl.SetFaceDir(faceDir);
        }
    }

    private void Attack() {
        if (!_canBehaviour) {
            return;
        }

        Mana = Mathf.Min(Mana + 10, _entityDecorator.Mana);

        //DoingAttack = true;

        AttackConfig config = Info.EntityAttackConfig;
        if (Mana == _entityDecorator.Mana) {
            // Use Skill
            Mana = 0;
            config = Info.EntitySkillConfig;
        }
        _animationControl.PlayAttackAnimation();

        var effects = config.attackEffects;
        List<EntityBase> targets 
            = Physics2D.OverlapCircleAll(transform.position, _entityDecorator.AttackRange + Radius * 2.5f, config.targetLayerMask)
                .Select(x => x.GetComponent<EntityBase>())
                .Where(x => x.Health > 0)
                .OrderBy(x => (x.transform.position - transform.position).sqrMagnitude)
                .ToList();

        if (targets.Count > 0) {
            Vector2 direction = (targets[0].transform.position - transform.position).normalized;
            HandDirection = direction;
            _animationControl.SetFaceDir(direction);

            SoundManager.Instance.PlayGameSe(config.soundEffectName);
            config.attackBehaviour.Behaviour(this, targets, effects);
        }
    }

    public void ReceiveDamage(int damage) {
        if (Health <= 0) return;

        Mana = Mathf.Min(Mana + 10, _entityDecorator.Mana);
        int finalDamage = Mathf.Max(1, damage - _entityDecorator.Block);
        Health -= finalDamage;
        if (Health <= 0) {
           OnEntityDead();
        }
    }

    private void OnEntityDead() {
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (IsUnloadCompleted && (other.CompareTag("Enemy") || other.CompareTag("Ally"))) {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            other.transform.position += direction * 100f * Time.deltaTime;
        }
    }
}
