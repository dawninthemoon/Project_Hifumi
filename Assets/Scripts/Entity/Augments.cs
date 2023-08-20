using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Augments", fileName = "NewAugment")]
public class Augments : ScriptableObject, IEntityStatus {
    [SerializeField] private int _health = 0;
    [SerializeField] private int _mana = 0;
    [SerializeField] private int _stress = 0;
    [SerializeField] private int _attackDamage = 0;
    [SerializeField] private int _attackSpeed = 0;
    [SerializeField] private int _moveSpeed = 0;
    [SerializeField] private int _attackRange = 0;
    
    public int Health { get { return _health; } }
    public int Mana { get { return _mana; } }
    public int Stress { get { return _stress; } }
    public int AttackDamage { get { return _attackDamage; } }
    public int AttackSpeed { get { return _attackSpeed; } }
    public int MoveSpeed { get { return _moveSpeed; } }
    public int AttackRange { get { return _attackRange; } }
}
