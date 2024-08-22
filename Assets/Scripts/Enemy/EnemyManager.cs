using System.Collections.Generic;

public class EnemyManager : Singleton<EnemyManager>
{
    List<EnemyController> enemyList = new();
    public List<EnemyController> EnemyList { get => enemyList; set { enemyList = value; } }

    protected override void Awake()
    {
        base.Awake();
    }

    public void AddEnemy(EnemyController enemy)
    {
        if(!enemyList.Contains(enemy))
            enemyList.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        if(enemyList.Contains(enemy))
            enemyList.Remove(enemy);
    }

}
