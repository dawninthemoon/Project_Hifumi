using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BuffInfo {
    [System.Serializable]
    public struct Pair<T> {
        public T value;
        public float durtaion;
    }
    public Pair<bool> stun;
    public float statusBuffDuration;
    public int health;
    public int mana;
    public int morale;
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
    public int AttackDamage { get { return _buffInfo.attackDamage; } }
    public int AttackSpeed { get { return _buffInfo.attackSpeed; } }
    public int MoveSpeed { get { return _buffInfo.moveSpeed; } }
    public int AttackRange { get { return _buffInfo.attackRange; } }
}