using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours {
    [CreateAssetMenu(fileName = "SplashEffectBehaviour", menuName = "ScriptableObjects/AttackBehaviours/SplashEffect")]
    public class SplashEffectBehaviour : AttackBehaviour {
        public override void Behaviour(EntityBase caster, List<EntityBase> targets, Effects.IAttackEffect[] effects) {
            // TODO: 이펙트 생기면 추가할 것
            /*
            var sonarEffect = Resources.Load<ParticleSystem>("Fx/fxSonar");
            sonarEffect = Instantiate(sonarEffect, caster.transform.position, Quaternion.identity);

            foreach (Effects.IAttackEffect effect in effects) {
                effect.ApplyEffect(caster, targets);
            }
            */
        }
    }
}
