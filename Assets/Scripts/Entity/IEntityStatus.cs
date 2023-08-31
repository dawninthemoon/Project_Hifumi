public interface IEntityStatus {
    int Health { get; }
    int Mana { get; }
    int Morale { get; }
    int AttackDamage { get; }
    float AttackSpeed { get; }
    int MoveSpeed { get; }
    int AttackRange { get; }
}