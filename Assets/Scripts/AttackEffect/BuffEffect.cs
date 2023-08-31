using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours.Effects {
    [CreateAssetMenu(fileName = "BuffEffect", menuName = "ScriptableObjects/AttackEffects/BuffEffect")]
    public class BuffEffect : AttackEffect {
        [SerializeField] private BuffConfig _buffConfig = null;
        public override void ApplyEffect(EntityBase caster, List<EntityBase> targets) {
            foreach (EntityBase target in targets) {
                target.BuffControl.StartApplyBuff(_buffConfig);
            }
        }
    }
}