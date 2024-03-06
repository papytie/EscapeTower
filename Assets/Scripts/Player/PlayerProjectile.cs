using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public HitboxShapeType HitboxShape => hitboxShape;
    public Color SetDebugColor { set { transform.GetComponentInChildren<SpriteRenderer>().color = value; } }
    public Vector2 HitboxOffset => hitboxOffset;
    public Vector2 BoxSize => boxSize;

    [Header("Projectile settings")]
    [SerializeField] float speed;
    [SerializeField] HitboxShapeType hitboxShape;
    [SerializeField] Vector2 hitboxOffset;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask obstructionLayer;
    [SerializeField] Vector2 boxSize;
    [SerializeField] float circleRadius;
    [SerializeField] int numberOfTarget;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color debugColor;
    [SerializeField] Mesh boxMesh;

    Vector2 startPosition = Vector2.zero;
    Vector2 endPosition = Vector2.zero;
    PlayerWeapon weapon;
    PlayerStats stats;
    float startTime = 0;

    List<EnemyLifeSystem> enemiesHit = new();

    public void Init(PlayerWeapon weaponRef, PlayerStats statsRef, Vector3 relativePos, float range)
    {
        //Ref
        weapon = weaponRef;
        stats = statsRef;

        //Init var
        startPosition = transform.position;
        endPosition = relativePos + transform.up * range;
        startTime = Time.time;
    }

    private void Update()
    {
        //Move gameObject
        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp01((Time.time - startTime) / (weapon.Range/speed)));

        //Define hitbox position with offset and check collisions
        Vector3 offsetPos = transform.position + (transform.right * hitboxOffset.x + transform.up * hitboxOffset.y);
        ProjectileHitProcess(offsetPos);
        CheckObstructionCollision(offsetPos);

        //Destroy if no collision at end of movement
        if (Time.time >= startTime + weapon.Range/speed)
        {
            Destroy(gameObject);
        }
    }

    bool ProjectileHitBoxCast(Vector2 position, out RaycastHit2D[] collisionsList)
    {
        collisionsList = hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(position, circleRadius, Vector2.zero, 0, enemyLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(position, boxSize, Quaternion.Angle(Quaternion.identity, transform.rotation), Vector2.zero, 0, enemyLayer),
            _ => null,
        };
        return collisionsList.Length > 0;
    }

    void CheckObstructionCollision(Vector2 position)
    {
        switch (hitboxShape)
        {
            case HitboxShapeType.Circle:
                if (Physics2D.CircleCast(position, circleRadius, Vector2.zero, 0, obstructionLayer))
                    Destroy(gameObject);
                break;

            case HitboxShapeType.Box:
                if (Physics2D.BoxCast(position, boxSize, Quaternion.Angle(Quaternion.identity, transform.rotation), Vector2.zero, 0, obstructionLayer))
                    Destroy(gameObject);
                break;
        }

    }

    void ProjectileHitProcess(Vector2 position)
    {
        if (ProjectileHitBoxCast(position, out RaycastHit2D[] collisionsList))
        {
            foreach (RaycastHit2D collision in collisionsList)
            {
                EnemyLifeSystem enemyLifesystem = collision.transform.GetComponent<EnemyLifeSystem>();

                if (enemyLifesystem && !enemyLifesystem.IsDead && !enemiesHit.Contains(enemyLifesystem) && enemiesHit.Count < numberOfTarget)
                {
                    enemyLifesystem.TakeDamage(stats.GetModifiedMainStat(MainStat.Damage));

                    //Call enemy Bump and give direction which is the inverted Normal of the collision
                    collision.transform.GetComponent<EnemyBump>().BumpedAwayActivation(-collision.normal);

                    enemiesHit.Add(enemyLifesystem);
                }

                //Destroy self if numberOfTarget is reached
                if (enemiesHit.Count >= numberOfTarget)
                    Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 offsetPos = transform.position + (transform.right * hitboxOffset.x + transform.up * hitboxOffset.y);

        if (showDebug)
        {
            Gizmos.color = debugColor;
            switch (hitboxShape)
            {
                case HitboxShapeType.Circle:
                    Gizmos.DrawWireSphere(offsetPos, circleRadius);
                    break;

                case HitboxShapeType.Box:
                    Gizmos.DrawWireMesh(boxMesh, -1, offsetPos, transform.rotation, boxSize);
                    break;

            }
        }

        if(Application.isPlaying && weapon.ShowDebug)
        {
            Gizmos.color = weapon.DebugColor;
            switch (hitboxShape)
            {
                case HitboxShapeType.Circle:
                    Gizmos.DrawSphere(transform.position, circleRadius);
                    break;

                case HitboxShapeType.Box:
                    Gizmos.DrawMesh(boxMesh, -1, offsetPos, transform.rotation, boxSize);
                    break;

            }

        }
    }
}
