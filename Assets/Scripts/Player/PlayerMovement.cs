using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float minDist = .1f;

    float CheckDistance = 0;

    PlayerInputs playerInputs;
    PlayerCollision playerCollision;
    Animator playerAnimator;

    public void InitRef(PlayerInputs inputRef, Animator animRef, PlayerCollision collisionRef)
    {
        playerInputs = inputRef;
        playerAnimator = animRef;
        playerCollision = collisionRef;
    }

    private void Start()
    {
        CheckDistance = minDist * 2;
    }

    public void Move()
    {
        Vector3 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();

        if (moveAxis != Vector3.zero)
        {
            playerCollision.MoveCheckCollision(moveAxis, CheckDistance, playerCollision.WallLayer, out RaycastHit2D hit);
            Vector3 playerMove = playerSpeed * Time.deltaTime * moveAxis;

            //Movement
            if (hit)
            {
                Vector3 stickyPos = hit.centroid + hit.normal * minDist;
                Vector3 stickyToInitial = (transform.position + playerMove) - stickyPos;
                Vector3 stickyAxis = Vector2.Perpendicular(hit.normal);

                //Projection of fixed player movement on Sticky Axis
                Vector3 projectedPos = stickyPos + stickyAxis * Vector2.Dot(stickyAxis, stickyToInitial);

                if (playerMove.magnitude > (projectedPos - transform.position).magnitude || hit.distance < minDist * .9f)
                    transform.position = stickyPos + stickyAxis * Vector2.Dot(stickyAxis, stickyToInitial);
            }
            else
            {
                transform.position += playerMove; //unrestricted Movement
            }
            
            //Rotation
            transform.rotation = Quaternion.LookRotation(Vector3.forward, moveAxis);

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

            }

        }
    }
}
