using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo : ObserverSubject {
    public EntityBase Self;
    public EntityBase Caster;
    public int FinalDamage;

    public DamageInfo(EntityBase self) {
        Self = self;
    }

    public void ReceiveDamage(int damage, EntityBase caster) {
        Caster = caster;
        FinalDamage = damage;
        Notify();
    }
}
