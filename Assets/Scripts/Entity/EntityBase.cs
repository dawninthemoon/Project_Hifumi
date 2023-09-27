using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityBase : MonoBehaviour {
    [SerializeField] private Transform _bulletPosition = null;
    private Agent _agent;
    private EntityAnimationControl _animationControl;
    private EntityDecorator _entityDecorator;
    private EntityHealthMana _healthManaControl;
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
        get { return _healthManaControl.Health; }
    }
    public int Mana { 
        get { return _healthManaControl.Mana; }
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
        _healthManaControl = new EntityHealthMana(GetComponent<EntityUIControl>());
        gameObject.AddComponent<DamageInfo>();

        BuffControl = new EntityBuff(this, _entityDecorator);

        _agent.OnMovementInput.AddListener(Move);
        _agent.OnAttackRequested.AddListener(Attack);
    }

    public void Initialize(EntityDecorator entityDecorator) {
        IsUnloadCompleted = false;
        _entityDecorator = entityDecorator;
        _bulletPosition.localPosition = Info.BulletOffset;

        _healthManaControl.Initialize(entityDecorator);
        _animationControl.Initialize(Info.BodySprite, Info.WeaponSprite, Info.AnimatorController);
        
        _agent.Initialize(_entityDecorator, Radius);

        InitalizeStatus();
    }

    private void Update() {
        if (BuffControl != null) {
            _canBehaviour = !BuffControl.IsDebuffExists("stun");
        }
    }

    private void InitalizeStatus() {
        _healthManaControl.Health = _entityDecorator.Health;
        _healthManaControl.Mana = 0;
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

        _healthManaControl.AddMana(10);

        AttackConfig config = Info.EntityAttackConfig;
        if (_healthManaControl.IsManaFull) {
            // Use Skill
            _healthManaControl.Mana = 0;
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

    public void ReceiveDamage(int damage, EntityBase caster = null) {
        if (Health <= 0) return;

        _healthManaControl.AddMana(10);
        int finalDamage = Mathf.Max(1, damage - _entityDecorator.Block);

        _healthManaControl.ReceiveDamage(finalDamage);
        GetComponent<DamageInfo>().ReceiveDamage(damage, this, caster);

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
