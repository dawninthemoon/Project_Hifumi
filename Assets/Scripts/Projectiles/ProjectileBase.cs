using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackBehaviours.Effects;
using CustomPhysics;

public abstract class ProjectileBase : MonoBehaviour {
    public abstract void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects);
    protected abstract void Update();
}
