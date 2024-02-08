using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float distFromCollider = .01f;

    PlayerInputs playerInputs;
    PlayerCollision playerCollision;
    Animator playerAnimator;

    public void InitRef(PlayerInputs inputRef, Animator animRef, PlayerCollision collisionRef)
    {
        playerInputs = inputRef;
        playerAnimator = animRef;
        playerCollision = collisionRef;
    }

    public void Move()
    {
        Vector3 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();

        if (playerCollision.CheckCollision(moveAxis, .1f, playerCollision.WallLayer, out Vector2 collisionPosition) && moveAxis != Vector3.zero)
        {
            Vector2 moveToStick = (collisionPosition - transform.position.ToVector2());
            Vector3 moveFinal = moveToStick.normalized * (moveToStick.magnitude - distFromCollider);
            
            transform.position += moveFinal;
        }
        else transform.position += playerSpeed * Time.deltaTime * moveAxis;

        if (moveAxis != Vector3.zero)
        {
            Quaternion rotateDirection = Quaternion.LookRotation(Vector3.forward, moveAxis);
            transform.rotation = rotateDirection;
        }

        playerAnimator.SetFloat(GameParams.Animation.PLAYER_RIGHTDAXIS_FLOAT, moveAxis.x);
        playerAnimator.SetFloat(GameParams.Animation.PLAYER_FORWARDAXIS_FLOAT, moveAxis.y);

    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Vector3 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
            playerCollision.CheckCollision(moveAxis, .1f, playerCollision.WallLayer, out Vector2 collisionPosition);
            Gizmos.DrawCube(transform.position + moveAxis * .1f, new Vector3(.5f, .6f, .5f));
            Gizmos.color = Color.white;

        }
    }
}
