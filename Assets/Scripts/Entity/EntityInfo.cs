using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

[CreateAssetMenu(menuName = "ScriptableObjects/EntityInfo", fileName = "NewEntityInfo")]
public class EntityInfo : ScriptableObject, IEntityStatus {
    [SerializeField] private Sprite _bodySprite = null;
    [SerializeField] private Sprite _weaponSprite = null;
    [SerializeField] private RuntimeAnimatorController _animatorController = null;

    [SerializeField] private int _health = 0;
    [SerializeField] private int _mana = 0;
    [SerializeField] private int _stress = 0;
    [SerializeField] private int _attackDamage = 0;
    [SerializeField] private int _attackSpeed = 0;
    [SerializeField] private int _moveSpeed = 0;
    [SerializeField] private int _attackRange = 0;

    public Sprite BodySprite { get { return _bodySprite; } }
    public Sprite WeaponSprite { get { return _weaponSprite; } }
    public RuntimeAnimatorController AnimatorController { get { return _animatorController; } }
    
    public int Health { get { return _health; } }
    public int Mana { get { return _mana; } }
    public int Stress { get { return _stress; } }
    public int AttackDamage { get { return _attackDamage; } }
    public int AttackSpeed { get { return _attackSpeed; } }
    public int MoveSpeed { get { return _moveSpeed; } }
    public int AttackRange { get { return _attackRange; } }
}
