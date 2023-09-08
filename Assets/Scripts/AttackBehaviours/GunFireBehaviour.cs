using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

namespace AttackBehaviours {
    [CreateAssetMenu(fileName = "NewGunFireBehaviour", menuName = "ScriptableObjects/AttackBehaviours/GunFire")]
    public class GunFireBehaviour : AttackBehaviour {
        [SerializeField] private string _projectilePrefabName = null;
        [SerializeField] private float _aimingEfficiency = 0f;
        [SerializeField] private int _bulletCount = 1;
        [SerializeField] private int _burstCount = 1;
        [SerializeField] private float _burstDelay = 0f;
        [SerializeField] private float _projectileSpeed = 80f;

        public override void Behaviour(EntityBase caster, List<EntityBase> targets, Effects.IAttackEffect[] effects) {
            if (targets.Count > 0) {
                caster.StartCoroutine(Fire(caster, targets, effects));
            }
        }

        private IEnumerator Fire(EntityBase caster, List<EntityBase> targets, Effects.IAttackEffect[] effects) {
            for (int burst = 0; burst < _burstCount; ++burst) {
                for (int i = 0; i < _bulletCount; ++i) {
                    SpawnBullet(caster, targets, effects);
                }
                yield return YieldInstructionCache.WaitForSeconds(_burstDelay);
            }
        }

        private void SpawnBullet(EntityBase caster, List<EntityBase> targets, Effects.IAttackEffect[] effects) {
            float defaultAngle = ExVector.GetDegree(caster.transform.position, targets[0].transform.position);
            float finalAngle = defaultAngle + Random.Range(-_aimingEfficiency, _aimingEfficiency);

            ProjectileBase projectile = ProjectileSpawner.Instance.GetProjectile(_projectilePrefabName);
            projectile.transform.position = caster.BulletPosition;
            projectile.Initialize(
                caster,
                targets[0],
                _projectileSpeed,
                effects,
                finalAngle
            );
        }
    }
}
