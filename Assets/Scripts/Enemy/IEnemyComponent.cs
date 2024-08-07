public interface IEnemyComponent
{
    EnemyController Controller { get; set; }
    void InitRef(EnemyController ctrlRef) 
    {
        Controller = ctrlRef;
    }
}