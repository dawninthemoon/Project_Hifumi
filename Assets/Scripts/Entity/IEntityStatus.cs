public interface IEntityStatus {
    int Health { get; }
    int Mana { get; }
    int Stress { get; }
    int AttackDamage { get; }
    int AttackSpeed { get; }
    int MoveSpeed { get; }
    int AttackRange { get; }
}