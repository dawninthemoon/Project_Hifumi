using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBehaviour : IAttackBehaviour {
    private int _damage = 5;
    public void Behaviour(EntityBase caster, EntityBase[] targetEntities) {
        foreach (EntityBase target in targetEntities) {
            
        }
    }
}
