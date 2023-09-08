using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BuffInfo {
    public float buffDuration;
    public int health;
    public int mana;
    public int morale;
    public int block;
    public int attackDamage;
    public int attackSpeed;
    public int moveSpeed;
    public int attackRange;
}

[CreateAssetMenu(fileName = "NewBuffConfig", menuName = "ScriptableObjects/BuffConfig")]
public class BuffConfig : ScriptableObject, IEntityStatus {
    [SerializeField] private BuffInfo _buffInfo;
    public BuffInfo Info {
        get { return _buffInfo; }
    }

    public int Health { get { return _buffInfo.health; } }
    public int Mana { get { return _buffInfo.mana; } }
    public int Morale { get { return _buffInfo.morale; } }
    public int Block { get { return _buffInfo.block; } }
    public int AttackDamage { get { return _buffInfo.attackDamage; } }
    public float AttackSpeed { get { return _buffInfo.attackSpeed; } }
    public int MoveSpeed { get { return _buffInfo.moveSpeed; } }
    public int AttackRange { get { return _buffInfo.attackRange; } }
}