using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours {
    [CreateAssetMenu(fileName = "ProjectileAttack", menuName = "ScriptableObjects/AttackBehaviours/ProjectileAttack")]
    public class ProjectileAttackBehaviour : AttackBehaviour {
        [SerializeField] private string _projectilePrefabName = null;
        [SerializeField] private int _projectileSpeed = 20;

        public override void Behaviour(EntityBase caster, List<EntityBase> targets, Effects.IAttackEffect[] effects) {
            if (targets.Count == 0) return;

            ProjectileBase projectile = ProjectileSpawner.Instance.GetProjectile(_projectilePrefabName);
            projectile.transform.position = caster.transform.position;
            projectile.Initialize(
                caster,
                targets[0],
                _projectileSpeed,
                effects
            );
        }
    }
}
