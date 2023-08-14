using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class EntityBase : MonoBehaviour {
    [SerializeField] private string _id = null;
    [SerializeField] private UICollider _uiCollider = null;
    [SerializeField] private SpriteRenderer _bodyRenderer = null;
    [SerializeField] private CircleCollider _bodyCollider = null;
    [SerializeField] private CircleCollider _attackRange = null;
    [SerializeField] private AttackConfig _attackConfig;
    [SerializeField] private AttackConfig _skillConfig;
    [SerializeField] private Transform _handTransform = null;
    [SerializeField] private Transform _hpBarTransform = null;
    [SerializeField] private Transform _mpBarTransform = null;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _maxMana = 100;
    [SerializeField] private int _attackDamage = 5;
    private Agent _agent;
    private Animator _animatorController;
    private List<EntityBase> _entitiesInAttackRange;
    private Vector2 _faceDir;
    public UICollider UICollider {
        get { return _uiCollider; }
    }
    public float Radius {
        get { return _bodyCollider.CircleShape.radius; }
    }
    public string ID {
        get { return _id; }
    }
    public bool DoingAttack { get; private set; }
    public int Health { get; private set; }
    public int Mana { get; private set; }
    public int AttackDamage { get { return _attackDamage; } }

    private void Awake() {
        _agent = new Agent(_bodyCollider, _moveSpeed);
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
        if (Health <= 0) return;

        var hpBarScale = _hpBarTransform.localScale;
        hpBarScale.x = (float)Health / _maxHealth * 0.7f;
        _hpBarTransform.localScale = hpBarScale;

        var mpBarScale = _mpBarTransform.localScale;
        mpBarScale.x = (float)Mana / _maxMana * 0.7f;
        _mpBarTransform.localScale = mpBarScale;

        Debug.DrawRay(transform.position, _faceDir * 10f, Color.cyan);
        _handTransform.rotation = VectorToQuaternion(_faceDir);
    }

    private void LateUpdate() {
        Attack();
        _entitiesInAttackRange.Clear();
        if (Health <= 0) {
            _bodyCollider.Layer = ColliderLayerMask.Obstacle;
            _attackRange.gameObject.SetActive(false);
        }
    }

    public void Move(EntityBase target) {
        var movedEntityInfo = _agent.Move(transform, target, _attackRange.CircleShape.radius);
        _faceDir = movedEntityInfo.Item1;
        SetMoveAnimationState(!movedEntityInfo.Item2);

        _bodyRenderer.flipX = (_faceDir.x < 0f);
        _handTransform.localScale = new Vector3(1f, Mathf.Sign(_faceDir.x), 1f);
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
        if (Health <= 0) return;

        Mana = Mathf.Min(Mana + 10, _maxMana);
        Health -= damage;
        if (Health <= 0) {
           OnEntityDead();
        }
    }

    public bool IsCollision(CustomCollider other) {
        return other.IsCollision(_bodyCollider);
    }

    private void OnEntityDead() {
         _animatorController.SetBool("isDead", true);
        _hpBarTransform.gameObject.SetActive(false);
        _mpBarTransform.gameObject.SetActive(false);
    }

    private void DisableAttackTrigger() {
        DoingAttack = false;
    }

    private Quaternion VectorToQuaternion(Vector2 direction) {
        float theta = Mathf.Atan2(direction.y, direction.x);
        return Quaternion.Euler(0f, 0f, theta * Mathf.Rad2Deg);
    }
}
