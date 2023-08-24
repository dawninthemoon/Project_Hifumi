using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatusDecorator : IEntityStatus {
    private EntityInfo _entityInfo;
    private List<Belongings> _belongingsList;

    public EntityStatusDecorator(EntityInfo entityInfo) {
        _belongingsList = new List<Belongings>();
        _entityInfo = entityInfo;
    }

    public void AddAugments(Belongings augment) {
        _belongingsList.Add(augment);
    }

    public void RemoveAugments(Belongings augment) {
        _belongingsList.Remove(augment);
    }

    public int Health { 
        get {
            int finalHealth = _entityInfo.Health;
            foreach (Belongings augments in _belongingsList) {
                finalHealth += augments.Health;
            } 
            return finalHealth;
        }
    }
    public int Mana { 
        get {
            int finalMana = _entityInfo.Mana;
            foreach (Belongings augments in _belongingsList) {
                finalMana += augments.Mana;
            } 
            return finalMana;
        }
    }
    public int Morale { 
        get {
            int finalStress = _entityInfo.Morale;
            foreach (Belongings augments in _belongingsList) {
                finalStress += augments.Morale;
            } 
            return finalStress;
        }
    }
    public int AttackDamage { 
        get {
            int finalDamage = _entityInfo.AttackDamage;
            foreach (Belongings augments in _belongingsList) {
                finalDamage += augments.AttackDamage;
            } 
            return finalDamage;
        }
    }
    public int AttackSpeed { 
        get {
            int finalAttackSpeed = _entityInfo.AttackSpeed;
            foreach (Belongings augments in _belongingsList) {
                finalAttackSpeed += augments.AttackSpeed;
            } 
            return finalAttackSpeed;
        }
    }
    public int MoveSpeed { 
        get {
            int finalMoveSpeed = _entityInfo.MoveSpeed;
            foreach (Belongings augments in _belongingsList) {
                finalMoveSpeed += augments.MoveSpeed;
            } 
            return finalMoveSpeed;
        }
    }

    public int AttackRange {
        get { 
            int finalAttackRange = _entityInfo.AttackRange;
            foreach (Belongings augments in _belongingsList) {
                finalAttackRange += augments.AttackRange;
            } 
            return finalAttackRange;
        }
    }
}
