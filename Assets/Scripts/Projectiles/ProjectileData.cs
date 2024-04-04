using System;
using UnityEngine;

[Serializable]
public class ProjectileData
{
    [Header("Projectile Option")]
    public bool useProjectile = false;
    public LayerMask obstructionLayer;
    public LayerMask projectileTargetLayer;
    public AnimationCurve launchCurve;
    public ProjectileReturnType projectileReturnType = ProjectileReturnType.NoReturn;
    public bool projectileReturnFlip = false;
    public AnimationCurve returnCurve;
    public ProjectileController projectileToSpawn;
    public Vector2 projectileSpawnOffset = Vector2.zero;
    public float projectileAngleOffset = 0;
    public int projectileNumber = 1;
    public ProjectileSpawnType projectileSpawnType = ProjectileSpawnType.AtOnce;
    public int projectileMaxTargets = 1;
    public float projectileSpeed = 1;
    public float spreadAngle = 60;
    public float projectileRange = 1;
}
