using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatusDecorator : IEntityStatus {
    private EntityInfo _entityInfo;
    private List<Augments> _augmentsList;

    public EntityStatusDecorator(EntityInfo entityInfo) {
        _augmentsList = new List<Augments>();
        _entityInfo = entityInfo;
    }

    public void AddAugments(Augments augment) {
        _augmentsList.Add(augment);
    }

    public void RemoveAugments(Augments augment) {
        _augmentsList.Remove(augment);
    }

    public int Health { 
        get {
            int finalHealth = _entityInfo.Health;
            foreach (Augments augments in _augmentsList) {
                finalHealth += augments.Health;
            } 
            return finalHealth;
        }
    }
    public int Mana { 
        get {
            int finalMana = _entityInfo.Mana;
            foreach (Augments augments in _augmentsList) {
                finalMana += augments.Mana;
            } 
            return finalMana;
        }
    }
    public int Stress { 
        get {
            int finalStress = _entityInfo.Stress;
            foreach (Augments augments in _augmentsList) {
                finalStress += augments.Stress;
            } 
            return finalStress;
        }
    }
    public int AttackDamage { 
        get {
            int finalDamage = _entityInfo.AttackDamage;
            foreach (Augments augments in _augmentsList) {
                finalDamage += augments.AttackDamage;
            } 
            return finalDamage;
        }
    }
    public int AttackSpeed { 
        get {
            int finalAttackSpeed = _entityInfo.AttackSpeed;
            foreach (Augments augments in _augmentsList) {
                finalAttackSpeed += augments.AttackSpeed;
            } 
            return finalAttackSpeed;
        }
    }
    public int MoveSpeed { 
        get {
            int finalMoveSpeed = _entityInfo.MoveSpeed;
            foreach (Augments augments in _augmentsList) {
                finalMoveSpeed += augments.MoveSpeed;
            } 
            return finalMoveSpeed;
        }
    }

    public int AttackRange {
        get { 
            int finalAttackRange = _entityInfo.AttackRange;
            foreach (Augments augments in _augmentsList) {
                finalAttackRange += augments.AttackRange;
            } 
            return finalAttackRange;
        }
    }
}
