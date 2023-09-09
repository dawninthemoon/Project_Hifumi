using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

[CreateAssetMenu(menuName = "ScriptableObjects/EntityInfo", fileName = "NewEntityInfo")]
public class EntityInfo : ScriptableObject, IEntityStatus {
    [SerializeField] private string _entityID = null;

    [SerializeField] private float _bodyRadius = 20f;
    [SerializeField] private Sprite _bodySprite = null;
    [SerializeField] private Sprite _weaponSprite = null;
    [SerializeField] private RuntimeAnimatorController _animatorController = null;

    [SerializeField] private int _health = 0;
    [SerializeField] private int _mana = 0;
    [SerializeField] private int _morale = 0;
    [SerializeField] private int _block = 0;
    [SerializeField] private int _attackDamage = 0;
    [SerializeField] private float _attackSpeed = 0;
    [SerializeField] private int _moveSpeed = 0;
    [SerializeField] private int _attackRange = 0;
    [SerializeField] private SynergyType _synergy1;
    [SerializeField] private SynergyType _synergy2;

    [SerializeField] private AttackConfig _attackConfig;
    [SerializeField] private AttackConfig _skillConfig;

    [SerializeField] private Vector2 _bulletOffset;

    public string EntityID { get { return _entityID; } }

    public float BodyRadius { get { return _bodyRadius; } }
    public Sprite BodySprite { get { return _bodySprite; } }
    public Sprite WeaponSprite { get { return _weaponSprite; } }
    public RuntimeAnimatorController AnimatorController { get { return _animatorController; } }
    
    public int Health { get { return _health; } }
    public int Mana { get { return _mana; } }
    public int Morale { get { return _morale; } }
    public int Block { get { return _block; } }
    public int AttackDamage { get { return _attackDamage; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    public int MoveSpeed { get { return _moveSpeed; } }
    public int AttackRange { get { return _attackRange; } }

    public SynergyType Synergy1 {
        get { return _synergy1; }
    }
    public SynergyType Synergy2 {
        get { return _synergy2; }
    }

    public AttackConfig EntityAttackConfig { get { return _attackConfig; } }
    public AttackConfig EntitySkillConfig { get { return _skillConfig; } }

    public Vector2 BulletOffset { get { return _bulletOffset; } }
}
