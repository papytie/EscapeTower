using UnityEngine;

public interface IAttackData
{
    public LayerMask TargetLayer { get; }

    public bool UseProjectile { get; }
    public LayerMask ObstructionLayer { get; }
    public AnimationCurve LaunchCurve { get; }
    public ProjectileReturnType ProjectileReturnType { get; }
    public bool ProjectileReturnFlip { get; }
    public AnimationCurve ReturnCurve { get; }
    public WeaponProjectile ProjectileToSpawn { get; }
    public Vector2 ProjectileSpawnOffset { get; }
    public float ProjectileAngleOffset { get; }
    public int ProjectileNumber { get; }
    public ProjectileSpawnType ProjectileSpawnType { get; }
    public int ProjectileMaxTargets { get; }
    public float ProjectileSpeed { get; }
    public float SpreadAngle { get; }
    public float ProjectileRange { get;}

}
