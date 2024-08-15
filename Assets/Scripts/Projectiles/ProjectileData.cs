using System;
using UnityEngine;

[Serializable]
public class ProjectileData
{
    [Header("Projectile Settings")]
    public ProjectileController projectileToSpawn;
    public Vector2 spawnOffset = Vector2.zero;
    public AnimationCurve launchCurve;
    public ProjectileSpawnType spawnType = ProjectileSpawnType.AtOnce;
    public float speed = 1;
    public float range = 1;
    public float angleOffset = 0;
    public int maxTargets = 1;
    public int spawnNumber = 1;
    public float spreadAngle = 60;

    [Header("Guided Settings")]
    public bool isGuided = false;
    public float lifespan = 5;

    [Header("Return Settings")]
    public bool returnFlip = false;
    public ProjectileReturnType returnType = ProjectileReturnType.NoReturn;
    public AnimationCurve returnCurve;

    [Header("Spawn")]
    public bool spawnObjectOnHit = false;
    public GameObject spawnObject;
}
