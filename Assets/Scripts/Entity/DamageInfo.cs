using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo : ObserverSubject {
    public EntityBase Self;
    public EntityBase Caster;
    public int FinalDamage;

    public void ReceiveDamage(int damage, EntityBase self, EntityBase caster) {
        Self = self;
        Caster = caster;
        FinalDamage = damage;
        Notify();
    }
}
