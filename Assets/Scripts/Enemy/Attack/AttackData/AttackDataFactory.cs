public class AttackDataFactory
{
    public static IAttackData CreateData(AttackType type)
    {
        return type switch
        {
            AttackType.Melee => new MeleeAttackData(),
            AttackType.Ranged => new RangedAttackData(),
            AttackType.Charge => new ChargeAttackData(),
            _ => null,
        };
    }
}
