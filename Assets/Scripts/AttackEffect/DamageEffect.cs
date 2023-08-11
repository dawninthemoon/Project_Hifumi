using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours.Effects {
    [CreateAssetMenu(fileName = "DamageEffect", menuName = "ScriptableObjects/AttackEffects/DamageEffect")]
    public class DamageEffect : AttackEffect {
        public override void ApplyEffect(EntityBase caster, List<EntityBase> targets) {
            foreach (EntityBase target in targets) {
                target.ReceiveDamage(caster.AttackDamage);
            }
        }
    }
}