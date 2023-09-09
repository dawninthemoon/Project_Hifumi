using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityBase : MonoBehaviour, IObserver {
    [SerializeField] private Transform _bulletPosition = null;
    private Agent _agent;
    private EntityInfo _entityInfo = null;
    private EntityAnimationControl _animationControl;
    private EntityUIControl _uiControl;
    private EntityStatusDecorator _statusDecorator;
    private int _currentHealth;
    private int _currentMana;
    private float _currentMorale;
    public EntityBuff BuffControl {
        get;
        private set;
    }
    public float Radius {
        get { return _entityInfo.BodyRadius; }
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
    public EntitySynergy Synergy1 {
        get { return _entityInfo.Synergy1; }
    }
    public EntitySynergy Synergy2 {
        get { return _entityInfo.Synergy2; }
    }

    public int AttackDamage { get { return _statusDecorator.AttackDamage; } }
    public Vector3 BulletPosition { get { return _bulletPosition.position; } }
    public bool IsUnloadCompleted { get; set; }
    private bool _canBehaviour = true;

    private void Awake() {
        _agent = GetComponent<Agent>();
        _animationControl = GetComponent<EntityAnimationControl>();
        _uiControl = GetComponent<EntityUIControl>();

        _statusDecorator = new EntityStatusDecorator();
        BuffControl = new EntityBuff(this, _statusDecorator);

        _agent.OnMovementInput.AddListener(Move);
        _agent.OnAttackRequested.AddListener(Attack);
    }

    public void Initialize(EntityInfo entityInfo) {
        IsUnloadCompleted = false;
        _entityInfo = entityInfo;
        _statusDecorator.Initialize(_entityInfo);
        _bulletPosition.localPosition = _entityInfo.BulletOffset;

        _animationControl.Initialize(_entityInfo.BodySprite, _entityInfo.WeaponSprite, _entityInfo.AnimatorController);
        
        _agent.Initialize(_statusDecorator, Radius);

        var belongingsList = GameMain.PlayerData.GetBelongingsList(_entityInfo.EntityID);
        _statusDecorator.BelongingsList = belongingsList;

        _uiControl.UpdateBelongingSprites(_statusDecorator.BelongingsList);

        InitalizeStatus();
    }

    public void Notify(ObserverSubject subject) {
        PlayerData data = subject as PlayerData;
        if (data == null) {
            return;
        }

        _uiControl.UpdateBelongingSprites(_statusDecorator.BelongingsList);
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
        _currentHealth = _statusDecorator.Health;
        _currentMana = 0;
        _currentMorale = _statusDecorator.Morale;
    }

    public void SetTarget(ITargetable target) {
        _agent.SetTarget(target);
    }

    private void Move(Vector2 direction) {
        if (!_canBehaviour) {
            return;
        }
        _animationControl.SetMoveAnimationState(!direction.Equals(Vector2.zero));
        if (direction.sqrMagnitude > 0f) {
            Vector3 nextPosition = transform.position + (Vector3)direction * Time.deltaTime * _statusDecorator.MoveSpeed;
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

        Mana = Mathf.Min(Mana + 10, _statusDecorator.Mana);

        //DoingAttack = true;

        AttackConfig config = _entityInfo.EntityAttackConfig;
        if (Mana == _statusDecorator.Mana) {
            // Use Skill
            Mana = 0;
            config = _entityInfo.EntitySkillConfig;
        }
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
        
        SoundManager.Instance.PlayGameSe(config.soundEffectName);
        config.attackBehaviour.Behaviour(this, targets, effects);
    }

    public void ReceiveDamage(int damage) {
        if (Health <= 0) return;

        Mana = Mathf.Min(Mana + 10, _statusDecorator.Mana);
        int finalDamage = Mathf.Max(1, damage - _statusDecorator.Block);
        Health -= finalDamage;
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
        if (IsUnloadCompleted && (other.CompareTag("Enemy") || other.CompareTag("Ally"))) {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            other.transform.position += direction * 100f * Time.deltaTime;
        }
    }
}
