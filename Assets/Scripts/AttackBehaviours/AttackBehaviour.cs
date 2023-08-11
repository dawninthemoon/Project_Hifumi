using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours {
    public interface IAttackBehaviour {
        void Behaviour(EntityBase caster, List<EntityBase> targets, Effects.IAttackEffect[] effect);
    }

    public abstract class AttackBehaviour : ScriptableObject, IAttackBehaviour {
        protected List<EntityBase> _cachedEntityList = new List<EntityBase>(1);
        public abstract void Behaviour(EntityBase caster, List<EntityBase> targets, Effects.IAttackEffect[] effect);
    }
}