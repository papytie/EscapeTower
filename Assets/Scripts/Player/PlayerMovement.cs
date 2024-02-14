using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float minDist = .1f;
    [SerializeField] float CheckDistance = .1f;

    Vector2 collisionPoint = Vector2.zero;
    Vector2 playerNoColMove = Vector2.zero;
    Vector2 vectorFromPointToPlayer = Vector2.zero;
    Vector2 hypotenus = Vector2.zero;
    Vector2 targetPos = Vector2.zero;
    Vector2 centroidPosition = Vector2.zero;
    Vector2 normalDir = Vector2.zero;
    Vector2 positionReduced = Vector2.zero;
    Vector2 playerToD1 = Vector2.zero;
    Vector2 playerToD2 = Vector2.zero;
    Vector2 playerToD3 = Vector2.zero;
    Vector2 debugD2ToD3 = Vector2.zero;
    Vector2 debugD2ToD1 = Vector2.zero;
    Vector2 debugD2ToCentroid = Vector2.zero;
    Vector2 debugPlayerToCentroid = Vector2.zero;
    Vector2 debugCentroidToPlayer = Vector2.zero;
    Vector2 debugCentroidToD2 = Vector2.zero;
    Vector2 debugFinalPos = Vector2.zero;

    bool isHit = false;

    PlayerInputs playerInputs;
    PlayerCollision playerCollision;
    Animator playerAnimator;

    public void InitRef(PlayerInputs inputRef, Animator animRef, PlayerCollision collisionRef)
    {
        playerInputs = inputRef;
        playerAnimator = animRef;
        playerCollision = collisionRef;
    }

    /*public void CheckedMove()
    {
        Vector3 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
        float playerStep = playerSpeed * Time.fixedDeltaTime; //Distance of player movement in 1 frame

        if (moveAxis != Vector3.zero)
        {
            //Movement
            if (playerCollision.MoveCheckCollision(moveAxis, playerStep, playerCollision.WallLayer, out Vector2 normal)) //first Cast = axis movement
            {
                Vector3 secondCheck = MovementVector2D(moveAxis) + normal * minDist; //Second check Vector
                if (playerCollision.MoveCheckCollision(secondCheck, playerStep, playerCollision.WallLayer, out Vector2 normal2)) //Second Cast
                {
                    Vector3 thirdCheck = MovementVector2D(secondCheck) + normal2 * minDist; //Third check Vector
                    if (playerCollision.MoveCheckCollision(thirdCheck, playerStep, playerCollision.WallLayer, out Vector2 normal3)) //Third Cast
                    {
                        return; //if third Cast fail player dont move
                    }
                    else transform.position += playerStep * thirdCheck.normalized; //no collision at third attempt move to thirdCheck position normalized
                }
                else transform.position += playerStep * secondCheck.normalized; //no collision at second attempt move to secondCheck position normalized
            }
            else transform.position += playerStep * moveAxis; //no collision at first attempt move to normal position

            //Rotation
            Quaternion rotateDirection = Quaternion.LookRotation(Vector3.forward, moveAxis);
            transform.rotation = rotateDirection;
        }
    }

    Vector2 MovementVector2D(Vector3 direction)
    {
        return playerSpeed * Time.deltaTime * direction;
    }*/

    public void NewCheckMove()
    {
        Vector2 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
        float playerStep = playerSpeed * Time.deltaTime; //Distance of player movement in 1 frame
        if (moveAxis != Vector2.zero) 
        {
            playerCollision.MoveCheckCollision(moveAxis, CheckDistance, playerCollision.WallLayer, out RaycastHit2D hit); //Check normal move
            //Movement
            if (hit)
            {
                //DEBUG
                isHit = true;
                collisionPoint = hit.point;
                centroidPosition = hit.centroid;
                //DEBUG

                Vector2 vectorFromPoint = transform.position.ToVector2() - hit.centroid; //Get Vector from hit.point to player.position
                vectorFromPointToPlayer = vectorFromPoint;
                Debug.Log("distance from point is :" + vectorFromPoint.magnitude);

                float collisionAngleFromNormal = Vector2.Angle(vectorFromPoint.normalized, hit.normal); //Get Angle between
                Debug.Log("Angle is :" + collisionAngleFromNormal);

                float hypLength = minDist / Mathf.Cos(collisionAngleFromNormal * Mathf.Deg2Rad);
                Debug.Log("hypotenus length is :" + hypLength);

                //DEBUG
                hypotenus = vectorFromPoint.normalized * hypLength;
                //DEBUG

                //transform.position = targetPos;
/*                if(vectorFromPoint.magnitude < hypLength)
                    transform.position = transform.position.ToVector2() + hit.centroid + hypotenus;
*/
 
            }
            else
            {
                isHit = false;
                transform.position = transform.position.ToVector2() + playerStep * moveAxis;
            }

            //Rotation
            Quaternion rotateDirection = Quaternion.LookRotation(Vector3.forward, moveAxis);
            transform.rotation = rotateDirection;
        }
    }

    public void CheckMove2()
    {

        Vector2 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
        float playerStep = playerSpeed * Time.deltaTime; //Distance of player movement in 1 frame
        if (moveAxis != Vector2.zero)
        {
            playerCollision.MoveCheckCollision(moveAxis, CheckDistance, playerCollision.WallLayer, out RaycastHit2D hit); //Check normal move
            
            if(hit)
            {
                isHit = true;

                Vector2 PlayerToD1 = playerSpeed * Time.deltaTime * moveAxis; 
                Vector2 PlayerToCentroid = hit.centroid - transform.position.ToVector2();
                Vector2 CentroidToPlayer = -PlayerToCentroid;
                Vector2 CentroidToNormalPos = hit.normal * minDist;
                float D2CNAngle = Vector2.Angle(CentroidToPlayer, CentroidToNormalPos);
                float HypD2CN = minDist / Mathf.Cos(D2CNAngle * Mathf.Deg2Rad);
                Vector2 CentroidToD2 = PlayerToCentroid.normalized * HypD2CN;
                Vector2 PlayerToD2 = PlayerToCentroid - CentroidToD2;
                Vector2 D2ToNormalPos = (hit.centroid + hit.normal * minDist) - (transform.position.ToVector2() + PlayerToD2);
                float MovementLength = Vector2.Dot(D2ToNormalPos.normalized, (playerSpeed * Time.deltaTime * moveAxis) - PlayerToD2);
                Vector2 D2ToFinalPos = D2ToNormalPos.normalized * MovementLength;
                Vector3 FinalPos = PlayerToD2 + D2ToFinalPos;            

                transform.position += FinalPos;

                //DEBUG
                debugFinalPos = FinalPos;
                debugCentroidToD2 = CentroidToD2;
                debugPlayerToCentroid = transform.position.ToVector2() + PlayerToCentroid;
                debugCentroidToPlayer = hit.centroid + CentroidToPlayer;
                playerNoColMove = transform.position.ToVector2() + playerStep * moveAxis;
                collisionPoint = hit.point;
                centroidPosition = hit.centroid;
                normalDir = hit.normal;
                playerToD1 = transform.position.ToVector2() + PlayerToD1;
                playerToD2 = transform.position.ToVector2() + PlayerToD2;
                //DEBUG
            }
            else
            {
                isHit = false;
                transform.position = transform.position.ToVector2() + playerStep * moveAxis;
            }

            //Rotation
            Quaternion rotateDirection = Quaternion.LookRotation(Vector3.forward, moveAxis);
            transform.rotation = rotateDirection;

        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (playerCollision.ShowDebug)
            {
                Gizmos.color = Color.yellow;
                Vector2 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
                Gizmos.DrawWireSphere(transform.position.ToVector2() + moveAxis * CheckDistance, playerCollision.ColliderRadius);
                Gizmos.color = Color.white;

                if(isHit)
                {
                    //PlayerMove
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(transform.position.ToVector2() + playerSpeed * Time.deltaTime * moveAxis, playerCollision.ColliderRadius);
                    Gizmos.color = Color.white;

                    //centroid pos
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(centroidPosition, playerCollision.ColliderRadius);
                    Gizmos.color = Color.white;

                    //normal + minDistPos
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(centroidPosition + normalDir * minDist, playerCollision.ColliderRadius);
                    Gizmos.color = Color.white;
                    
                    //PlayerToD2
                    Gizmos.color = Color.cyan;           
                    Gizmos.DrawLine(transform.position, playerToD2);
                    Gizmos.DrawWireSphere(playerToD2, playerCollision.ColliderRadius);
                    Gizmos.color = Color.white;

                    Gizmos.color = Color.blue;           
                    Gizmos.DrawLine(transform.position, playerToD2);
                    Gizmos.DrawWireSphere(transform.position.ToVector2() + debugFinalPos, playerCollision.ColliderRadius);
                    Gizmos.color = Color.white;

                }

            }

        }
    }
}
