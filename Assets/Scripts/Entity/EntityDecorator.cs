using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDecorator : IEntityStatus {
    private List<BuffConfig> _buffList;
    public EntityInfo Info {
        get;
        private set;
    }
    public Belongings Item {
        get;
        set;
    }

    public EntityDecorator(EntityInfo entityInfo) {
        _buffList = new List<BuffConfig>();
        Info = entityInfo;
    }

    public void Initialize(EntityInfo entityInfo) {
        Info = entityInfo;
        _buffList.Clear();
    }

    public void AddBuff(BuffConfig config) {
        _buffList.Add(config);
    }

    public void RemoveBuff(BuffConfig config) {
        _buffList.Remove(config);
    }

    public int Health { 
        get {
            int finalHealth = Info.Health;

            if (Item)
                finalHealth += Item.Health;
            foreach (IEntityStatus buff in _buffList) {
                finalHealth += buff.Health;
            }
            return finalHealth;
        }
    }
    public int Mana { 
        get {
            int finalMana = Info.Mana;

            if (Item)
                finalMana += Item.Mana;
            foreach (IEntityStatus buff in _buffList) {
                finalMana += buff.Mana;
            }
            return finalMana;
        }
    }
    public int Morale { 
        get {
            int finalMorale = Info.Morale;

            if (Item)
                finalMorale += Item.Morale;
            foreach (IEntityStatus buff in _buffList) {
                finalMorale += buff.Morale;
            }
            return finalMorale;
        }
    }
    public int Block {
        get {
            int finalBlock = Info.Block;

            if (Item)
                finalBlock += Item.Block;
            foreach (IEntityStatus buff in _buffList) {
                finalBlock += buff.Block;
            }
            return finalBlock;
        }
    }
    public int AttackDamage { 
        get {
            int finalDamage = Info.AttackDamage;
            
            float damageMultiplier = 0f;

            if (Item)
                finalDamage += Item.AttackDamage;
            foreach (IEntityStatus buff in _buffList) {
                finalDamage += buff.AttackDamage;
            }
            foreach (BuffConfig buff in _buffList) {
                damageMultiplier += buff.AttackDamagePercent;
            }
            finalDamage += Mathf.FloorToInt(finalDamage * damageMultiplier);
            return finalDamage;
        }
    }
    public float AttackSpeed { 
        get {
            float finalAttackSpeed = Info.AttackSpeed;
            float attackSpeedMultiplier = 0f;

            if (Item)
                attackSpeedMultiplier += Item.AttackSpeed;
            foreach (IEntityStatus buff in _buffList) {
                attackSpeedMultiplier += buff.AttackSpeed;
            }
            finalAttackSpeed += Info.AttackSpeed * attackSpeedMultiplier;
            return finalAttackSpeed;
        }
    }
    public int MoveSpeed { 
        get {
            int finalMoveSpeed = Info.MoveSpeed;

            if (Item)
                finalMoveSpeed += Item.MoveSpeed;
            foreach (IEntityStatus buff in _buffList) {
                finalMoveSpeed += buff.MoveSpeed;
            }
            return finalMoveSpeed;
        }
    }

    public int AttackRange {
        get { 
            int finalAttackRange = Info.AttackRange;

            if (Item)
                finalAttackRange += Item.AttackRange;
            foreach (IEntityStatus buff in _buffList) {
                finalAttackRange += buff.AttackRange;
            }
            return finalAttackRange;
        }
    }

    public float AimingEfficiency {
        get { 
            float finalAiming = 0f;
            foreach (BuffConfig buff in _buffList) {
                finalAiming += buff.AimingEfficiency;
            }
            return finalAiming;
        }
    }

    public SynergyType ExtraSynergy {
        get {
            SynergyType extraSynergy = SynergyType.None;
            if (Item && !Item.ExtraSynergy.Equals(SynergyType.None)) {
                extraSynergy = Item.ExtraSynergy;
            }
            return extraSynergy;
        }
    }
}
