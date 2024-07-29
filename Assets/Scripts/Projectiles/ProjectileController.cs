using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class ProjectileController : MonoBehaviour
{
    public HitboxShapeType HitboxShape => hitboxShape;
    public Color SetDebugColor { set { transform.GetComponentInChildren<SpriteRenderer>().color = value; } }
    public Vector2 HitboxOffset => hitboxOffset;
    public Vector2 BoxSize => boxSize;
    public float CircleRadius => circleRadius;

    [Header("Projectile settings")]
    [SerializeField] HitboxShapeType hitboxShape = HitboxShapeType.Circle;
    [SerializeField] Vector2 hitboxOffset = Vector2.zero;
    [SerializeField] Vector2 boxSize = new(0.1f, 0.1f);
    [SerializeField] float circleRadius = .1f;
    [SerializeField] float falloffDelay = .1f;
    [SerializeField] float obstructionDelay = .1f;
    [SerializeField] float hitDelay = .1f;

    [Header("Debug")]
    [SerializeField] bool showDebug = true;
    [SerializeField] Color debugColor = Color.red;
    [SerializeField] Mesh boxMesh;

    Vector2 startPosition = Vector2.zero;
    Vector2 endPosition = Vector2.zero;
    float startTime = 0;
    float damage = 0;
    bool isReturning = false;
    bool hasHit = false;
    GameObject owner;
    ProjectileData data;
    Animator animator;

    List<ILifeSystem> hitList = new();

    public void Init(GameObject projectileOwner, ProjectileData data, Vector3 relativePos, float projectileDamage, float projectileRange)
    {
        owner = projectileOwner;
        this.data = data;
        damage = projectileDamage;
        endPosition = relativePos + transform.up * projectileRange;
        startPosition = transform.position;
        startTime = Time.time;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isReturning && !hasHit)
            ProjectileMovement(startPosition, endPosition);

        if (isReturning)
        {
            switch (data.returnType)
            {
                case ProjectileReturnType.ReturnToSpawnPosition:
                    ProjectileMovement(startPosition, endPosition);
                    break;
                case ProjectileReturnType.ReturnToPlayer:
                    ProjectileMovement(startPosition, owner.transform.position);
                    break;

            }
        }

        if (Time.time >= startTime + data.range / data.speed)
        {
            if(!isReturning && data.returnType != ProjectileReturnType.NoReturn)
            {
                endPosition = startPosition;
                startPosition = transform.position;
                startTime = Time.time;
                isReturning = true;
                return;
            }
            animator.SetTrigger(SRAnimators.ProjectileAnimBase.Parameters.falloff);
            Invoke(nameof(DestroyProjectile), falloffDelay);
        }
    }

    void ProjectileMovement(Vector2 startPos, Vector2 endPos)
    {
        float t = Mathf.Clamp01((Time.time - startTime) / (data.range / data.speed));
        //Move gameObject
        transform.position = Vector3.Lerp(startPos, endPos, data.launchCurve.Evaluate(t));

        if(isReturning && data.returnType == ProjectileReturnType.ReturnToPlayer)
        {
            Vector3 toPlayerVector = owner.transform.position - transform.position;
            if (data.returnFlip)
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
            HitboxShapeType.Circle => Physics2D.CircleCastAll(position, circleRadius, Vector2.zero, 0, data.targetLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(position, boxSize, Quaternion.Angle(Quaternion.identity, transform.rotation), Vector2.zero, 0, data.targetLayer),
            _ => null,
        };
        return collisionsList.Length > 0;
    }

    void CheckObstructionCollision(Vector2 position)
    {
        switch (hitboxShape)
        {
            case HitboxShapeType.Circle:
                RaycastHit2D circleCollision = Physics2D.CircleCast(position, circleRadius, Vector2.zero, 0, data.obstructionLayer);
                if (circleCollision)
                {
                    hasHit = true;
                    RotateInCollisionNormalDirection(circleCollision);
                    animator.SetTrigger(SRAnimators.ProjectileAnimBase.Parameters.obstructed);
                    Invoke(nameof(DestroyProjectile), obstructionDelay);
                }
                break;

            case HitboxShapeType.Box:
                RaycastHit2D boxCollision = Physics2D.BoxCast(position, boxSize, Quaternion.Angle(Quaternion.identity, transform.rotation), Vector2.zero, 0, data.obstructionLayer);
                if (boxCollision)
                {
                    hasHit = true;
                    RotateInCollisionNormalDirection(boxCollision);
                    animator.SetTrigger(SRAnimators.ProjectileAnimBase.Parameters.obstructed);
                    Invoke(nameof(DestroyProjectile), obstructionDelay);
                }
                break;

        }

    }

    void ProjectileHitProcess(Vector2 position)
    {
        if (ProjectileHitBoxCast(position, out RaycastHit2D[] collisionsList))
        {
            foreach (RaycastHit2D collision in collisionsList)
            {
                if (!collision.transform.TryGetComponent<ILifeSystem>(out var lifeSystem)) return;

                if (!lifeSystem.IsDead && !hitList.Contains(lifeSystem) && hitList.Count < data.maxTargets)
                {
                    //Animation
                    animator.SetTrigger(SRAnimators.ProjectileAnimBase.Parameters.hit);       
                    lifeSystem.TakeDamage(damage, Vector2.zero);
                    hitList.Add(lifeSystem);
                }

                //Destroy self if numberOfTarget is reached
                if (hitList.Count >= data.maxTargets)
                {
                    hasHit = true;
                    Invoke(nameof(DestroyProjectile), hitDelay);
                }
            }

            if (data.spawnObjectOnHit)
                Instantiate(data.spawnObject, transform.position, Quaternion.identity);
        }
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    void RotateInCollisionNormalDirection(RaycastHit2D hit)
    {
        Quaternion collisionRotation = new();
        collisionRotation.SetFromToRotation(transform.up, -hit.normal);
        transform.rotation *= collisionRotation;

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
