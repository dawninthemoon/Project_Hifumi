using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackBehaviours.Effects;
using RieslingUtils;

public class SimpleProjectile : ProjectileBase {
    [SerializeField] private float _lifeTime = 20f;
    private string _targetTag;
    private Vector3 _direction;
    private float _moveSpeed;
    private float _timeAgo;
    private EntityBase _caster;
    private IAttackEffect[] _effects;
    private List<EntityBase> _cachedEntityList = new List<EntityBase>(1);
    public override void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects) {
        _targetTag = target.tag;
        _caster = caster;
        _effects = effects;
        _direction = (target.transform.position - transform.position).normalized;
        float angle = ExVector.GetDegree(target.transform.position, transform.position);
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        _moveSpeed = moveSpeed;
        _timeAgo = 0f;
    }

    protected override void Update() {
        Vector3 moveVector = _direction * _moveSpeed * Time.deltaTime;
        transform.position += moveVector;

        _timeAgo += Time.deltaTime;
        if (_timeAgo > _lifeTime) {
            ProjectileSpawner.Instance.RemoveProjectile(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(_targetTag)) {
            _cachedEntityList.Clear();
            _cachedEntityList.Add(other.GetComponent<EntityBase>());

            foreach (IAttackEffect effect in _effects) {
                effect.ApplyEffect(_caster, _cachedEntityList);
            }

            ProjectileSpawner.Instance.RemoveProjectile(this);
        }
    }
}
