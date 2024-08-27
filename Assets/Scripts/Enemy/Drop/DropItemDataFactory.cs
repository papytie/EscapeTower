public class DropItemDataFactory
{
    public static IDropItemData Create(DropType type)
    {
        return (type) switch
        {
            //DropType.Timer => new DropTimerData(),
            DropType.Damage => new DropDamageData(),
            DropType.Die => new DropDieData(),
            _ => null,
        };
    }
}
