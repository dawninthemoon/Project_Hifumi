using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours {
    [CreateAssetMenu(fileName = "ProjectileAttack", menuName = "ScriptableObjects/AttackBehaviours/ProjectileAttack")]
    public class ProjectileAttackBehaviour : AttackBehaviour {
        [SerializeField] private ProjectileBase _projectilePrefab = null;

        public override void Behaviour(EntityBase caster, List<EntityBase> targets, Effects.IAttackEffect[] effects) {
            if (targets.Count == 0) return;

            var projectile = Instantiate(_projectilePrefab, caster.transform.position, Quaternion.identity);
            projectile.Initialize(caster, targets[0], 5f, effects);
        }
    }
}
