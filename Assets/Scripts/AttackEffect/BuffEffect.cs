using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours.Effects {
    [CreateAssetMenu(fileName = "NewBuffEffect", menuName = "ScriptableObjects/AttackEffects/BuffEffect")]
    public class BuffEffect : AttackEffect {
        [SerializeField] private BuffConfig _buffConfig = null;
        [SerializeField] private DebuffConfig _debuffConfig = null;
        public override void ApplyEffect(EntityBase caster, List<EntityBase> targets) {
            if (_buffConfig != null) {
                foreach (EntityBase target in targets) {
                    target.BuffControl.AddBuffWithDuration(_buffConfig);
                }
            }
            if (_debuffConfig != null) {
                foreach (EntityBase target in targets) {
                    target.BuffControl.StartAddDebuff(_debuffConfig);
                }
            }
        }
    }
}