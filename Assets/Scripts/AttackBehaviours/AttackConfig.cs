using AttackBehaviours;
using AttackBehaviours.Effects;
using UnityEngine;

[System.Serializable]
public struct AttackConfig {
    public LayerMask targetLayerMask;
    public AttackBehaviour attackBehaviour;
    public AttackEffect[] attackEffects;
    public string soundEffectName;
}
