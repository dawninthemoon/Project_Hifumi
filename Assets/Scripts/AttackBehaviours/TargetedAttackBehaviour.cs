using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours {
    [CreateAssetMenu(fileName = "NewTargetedAttackBehaviour", menuName = "ScriptableObjects/AttackBehaviours/TargetedAttack")]
    public class TargetedAttackBehaviour : AttackBehaviour {
        public override void Behaviour(EntityBase caster, List<EntityBase> targets, Effects.IAttackEffect[] effects) {
            if (targets.Count == 0) return;
            _cachedEntityList.Clear();
            _cachedEntityList.Add(targets[0]);

            foreach (Effects.IAttackEffect effect in effects) {
                effect.ApplyEffect(caster, _cachedEntityList);
            }
        }
    }
}
