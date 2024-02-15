using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;

    PlayerInputs playerInputs;
    PlayerCollision playerCollision;
    Animator playerAnimator;

    public void InitRef(PlayerInputs inputRef, Animator animRef, PlayerCollision collisionRef)
    {
        playerInputs = inputRef;
        playerAnimator = animRef;
        playerCollision = collisionRef;
    }

    public void Move2()
    {
        Vector3 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
        float playerStep = Time.deltaTime * playerSpeed;

        if (moveAxis != Vector3.zero)
        {
            playerCollision.CollisionCheck(moveAxis, playerStep, playerCollision.WallLayer, out Vector3 finalPosition);

            //Movement
            transform.position = finalPosition;
            
            //Rotation
            transform.rotation = Quaternion.LookRotation(Vector3.forward, moveAxis);
        }

    }

}
