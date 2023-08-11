using UnityEngine;
using System.Collections.Generic;

namespace AttackBehaviours.Effects {
    public interface IAttackEffect {
        void ApplyEffect(EntityBase caster, List<EntityBase> targets);
    }

    public abstract class AttackEffect : ScriptableObject, IAttackEffect {
        public abstract void ApplyEffect(EntityBase caster, List<EntityBase> targets);
    }
}