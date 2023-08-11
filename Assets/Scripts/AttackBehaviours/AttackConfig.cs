using AttackBehaviours;
using AttackBehaviours.Effects;

[System.Serializable]
public struct AttackConfig {
    public AttackBehaviour attackBehaviour;
    public AttackEffect[] attackEffects;
}
