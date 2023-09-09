using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Belongings", fileName = "NewBelonging")]
public class Belongings : ScriptableObject, IEntityStatus {
    [SerializeField] private Sprite _sprite = null;

    [SerializeField] private int _health = 0;
    [SerializeField] private int _mana = 0;
    [SerializeField] private int _morale = 0;
    [SerializeField] private int _block = 0;
    [SerializeField] private int _attackDamage = 0;
    [SerializeField] private float _attackSpeed = 0f;
    [SerializeField] private int _moveSpeed = 0;
    [SerializeField] private int _attackRange = 0;
    [SerializeField] private SynergyType _extraSynergy = SynergyType.None;
    
    public int Health { get { return _health; } }
    public int Mana { get { return _mana; } }
    public int Morale { get { return _morale; } }
    public int Block { get { return _block; } }
    public int AttackDamage { get { return _attackDamage; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    public int MoveSpeed { get { return _moveSpeed; } }
    public int AttackRange { get { return _attackRange; } }
    public SynergyType ExtraSynergy { get { return _extraSynergy; } }

    public Sprite Sprite { get { return _sprite; } }
}
