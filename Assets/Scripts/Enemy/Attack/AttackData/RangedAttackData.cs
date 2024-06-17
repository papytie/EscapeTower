using System;

[Serializable]
public class RangedAttackData : IAttackData
{
    public int priority = 0;
    public float cooldown = .5f;
    public float lag = .2f;
    public float delay = 0;
    public float duration = .1f;
    public ProjectileData projectileData;
}
