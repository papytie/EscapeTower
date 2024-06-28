using System;

[Serializable]
public class RangedData : IActionData
{
    public float cooldown = .5f;
    public float attackDuration = .2f;
    public float delay = 0;
    public ProjectileData projectileData;
}
