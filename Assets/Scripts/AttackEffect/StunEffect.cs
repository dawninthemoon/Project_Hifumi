using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours.Effects {
    [CreateAssetMenu(fileName = "StunEffect", menuName = "ScriptableObjects/AttackEffects/StunEffect")]
    public class StunEffect : AttackEffect {
        [SerializeField] private float _duration = 1f;
        public override void ApplyEffect(EntityBase caster, List<EntityBase> targets) {
            foreach (EntityBase target in targets) {
                target.ApplyStun(_duration);
            }
        }
    }
}