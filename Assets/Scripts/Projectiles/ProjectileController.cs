using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public HitboxShapeType HitboxShape => hitboxShape;
    public Color SetDebugColor { set { transform.GetComponentInChildren<SpriteRenderer>().color = value; } }
    public Vector2 HitboxOffset => hitboxOffset;
    public Vector2 BoxSize => boxSize;
    public float CircleRadius => circleRadius;

    [Header("Projectile settings")]
    [SerializeField] HitboxShapeType hitboxShape;
    [SerializeField] Vector2 hitboxOffset;
    [SerializeField] Vector2 boxSize;
    [SerializeField] float circleRadius;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color debugColor;
    [SerializeField] Mesh boxMesh;

    Vector2 startPosition = Vector2.zero;
    Vector2 endPosition = Vector2.zero;
    float startTime = 0;
    float damage = 0;
    bool isReturning = false;
    GameObject owner;
    ProjectileData projData;

    List<ILifeSystem> hitList = new();

    public void Init(GameObject projectileOwner, ProjectileData data, Vector3 relativePos, float projectileDamage)
    {
        owner = projectileOwner;
        projData = data;
        damage = projectileDamage;
        endPosition = relativePos + transform.up * projData.projectileRange;
        startPosition = transform.position;
        startTime = Time.time;
    }

    private void Update()
    {
        if (!isReturning)
            ProjectileMovement(startPosition, endPosition);

        if (isReturning)
        {
            switch (projData.projectileReturnType)
            {
                case ProjectileReturnType.ReturnToSpawnPosition:
                    ProjectileMovement(startPosition, endPosition);
                    break;
                case ProjectileReturnType.ReturnToPlayer:
                    ProjectileMovement(startPosition, owner.transform.position);
                    break;

            }
        }

        if (Time.time >= startTime + projData.projectileRange / projData.projectileSpeed)
        {
            if(!isReturning && projData.projectileReturnType != ProjectileReturnType.NoReturn)
            {
                endPosition = startPosition;
                startPosition = transform.position;
                startTime = Time.time;
                isReturning = true;
                return;
            }
            Destroy(gameObject);
            
        }

    }

    void ProjectileMovement(Vector2 startPos, Vector2 endPos)
    {
        float t = Mathf.Clamp01((Time.time - startTime) / (projData.projectileRange / projData.projectileSpeed));
        //Move gameObject
        transform.position = Vector3.Lerp(startPos, endPos, projData.launchCurve.Evaluate(t));

        if(isReturning && projData.projectileReturnType == ProjectileReturnType.ReturnToPlayer)
        {
            Vector3 toPlayerVector = owner.transform.position - transform.position;
            if (projData.projectileReturnFlip)
                transform.rotation = Quaternion.FromToRotation(Vector3.up, toPlayerVector.normalized);
            else transform.rotation = Quaternion.FromToRotation(Vector3.up, -toPlayerVector.normalized);
        }

        //Define hitbox position with offset and check collisions
        Vector3 offsetPos = transform.position + (transform.right * hitboxOffset.x + transform.up * hitboxOffset.y);
        ProjectileHitProcess(offsetPos);
        CheckObstructionCollision(offsetPos);
    }

    bool ProjectileHitBoxCast(Vector2 position, out RaycastHit2D[] collisionsList)
    {
        collisionsList = hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(position, circleRadius, Vector2.zero, 0, projData.projectileTargetLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(position, boxSize, Quaternion.Angle(Quaternion.identity, transform.rotation), Vector2.zero, 0, projData.projectileTargetLayer),
            _ => null,
        };
        return collisionsList.Length > 0;
    }

    void CheckObstructionCollision(Vector2 position)
    {
        switch (hitboxShape)
        {
            case HitboxShapeType.Circle:
                if (Physics2D.CircleCast(position, circleRadius, Vector2.zero, 0, projData.obstructionLayer))
                    Destroy(gameObject);
                break;

            case HitboxShapeType.Box:
                if (Physics2D.BoxCast(position, boxSize, Quaternion.Angle(Quaternion.identity, transform.rotation), Vector2.zero, 0, projData.obstructionLayer))
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
                ILifeSystem lifeSystem = collision.transform.GetComponent<ILifeSystem>();
                if (lifeSystem == null) return;

                if (!lifeSystem.IsDead && !hitList.Contains(lifeSystem) && hitList.Count < projData.projectileMaxTargets)
                {
                    lifeSystem.TakeDamage(damage);

                    if(lifeSystem is EnemyLifeSystemComponent)
                    {
                        //Call enemy Bump and give direction which is the inverted Normal of the collision
                        collision.transform.GetComponent<EnemyBumpComponent>().BumpedAwayActivation(-collision.normal);
                    }

                    hitList.Add(lifeSystem);
                }

                //Destroy self if numberOfTarget is reached
                if (hitList.Count >= projData.projectileMaxTargets)
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

        if(Application.isPlaying && showDebug)
        {
            Gizmos.color = debugColor;
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