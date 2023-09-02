using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatusDecorator : IEntityStatus {
    private EntityInfo _entityInfo;
    private List<BuffConfig> _buffList;
    public List<Belongings> BelongingsList {
        get;
        set;
    }

    public EntityStatusDecorator() {
        BelongingsList = new List<Belongings>();
        _buffList = new List<BuffConfig>();
    }

    public void Initialize(EntityInfo entityInfo) {
        _entityInfo = entityInfo;
    }
    public void AddBelongings(Belongings belongings) {
        BelongingsList.Add(belongings);
    }

    public void RemoveBelongings(Belongings belongings) {
        BelongingsList.Remove(belongings);
    }

    public void AddBuff(BuffConfig config) {
        _buffList.Add(config);
    }

    public void RemoveBuff(BuffConfig config) {
        _buffList.Remove(config);
    }

    public int Health { 
        get {
            int finalHealth = _entityInfo.Health;
            foreach (Belongings augments in BelongingsList) {
                finalHealth += augments.Health;
            }
            foreach (IEntityStatus buff in _buffList) {
                finalHealth += buff.Health;
            }
            return finalHealth;
        }
    }
    public int Mana { 
        get {
            int finalMana = _entityInfo.Mana;
            foreach (Belongings augments in BelongingsList) {
                finalMana += augments.Mana;
            }
            foreach (IEntityStatus buff in _buffList) {
                finalMana += buff.Mana;
            }
            return finalMana;
        }
    }
    public int Morale { 
        get {
            int finalStress = _entityInfo.Morale;
            foreach (Belongings augments in BelongingsList) {
                finalStress += augments.Morale;
            }
            foreach (IEntityStatus buff in _buffList) {
                
            }
            return finalStress;
        }
    }
    public int AttackDamage { 
        get {
            int finalDamage = _entityInfo.AttackDamage;
            foreach (Belongings augments in BelongingsList) {
                finalDamage += augments.AttackDamage;
            }
            foreach (IEntityStatus buff in _buffList) {
                finalDamage += buff.AttackDamage;
            }
            return finalDamage;
        }
    }
    public float AttackSpeed { 
        get {
            float finalAttackSpeed = _entityInfo.AttackSpeed;
            foreach (Belongings augments in BelongingsList) {
                finalAttackSpeed += augments.AttackSpeed;
            }
            foreach (IEntityStatus buff in _buffList) {
                finalAttackSpeed += buff.AttackSpeed;
            }
            return finalAttackSpeed;
        }
    }
    public int MoveSpeed { 
        get {
            int finalMoveSpeed = _entityInfo.MoveSpeed;
            foreach (Belongings augments in BelongingsList) {
                finalMoveSpeed += augments.MoveSpeed;
            }
            foreach (IEntityStatus buff in _buffList) {
                finalMoveSpeed += buff.MoveSpeed;
            }
            return finalMoveSpeed;
        }
    }

    public int AttackRange {
        get { 
            int finalAttackRange = _entityInfo.AttackRange;
            foreach (Belongings augments in BelongingsList) {
                finalAttackRange += augments.AttackRange;
            }
            foreach (IEntityStatus buff in _buffList) {
                finalAttackRange += buff.AttackRange;
            }
            return finalAttackRange;
        }
    }
}
