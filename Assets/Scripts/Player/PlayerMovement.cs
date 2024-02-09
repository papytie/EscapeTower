using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float minDist = .1f;

    PlayerInputs playerInputs;
    PlayerCollision playerCollision;
    Animator playerAnimator;

    public void InitRef(PlayerInputs inputRef, Animator animRef, PlayerCollision collisionRef)
    {
        playerInputs = inputRef;
        playerAnimator = animRef;
        playerCollision = collisionRef;
    }

    public void CheckedMove()
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
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (playerCollision.ShowDebug)
            {
                Gizmos.color = Color.yellow;
                Vector3 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
                float playerStep = playerSpeed * Time.fixedDeltaTime;
                Gizmos.DrawSphere(transform.position + moveAxis * playerStep, playerCollision.ColliderRadius);
                Gizmos.color = Color.white;

            }

        }
    }
}
