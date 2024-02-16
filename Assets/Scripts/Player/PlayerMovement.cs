using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float playerAcceleration = 50f;
    float currentSpeed = 0;

    PlayerInputs playerInputs;
    PlayerCollision playerCollision;
    Animator playerAnimator;

    public void InitRef(PlayerInputs inputRef, Animator animRef, PlayerCollision collisionRef)
    {
        playerInputs = inputRef;
        playerAnimator = animRef;
        playerCollision = collisionRef;
    }

    private void Update()
    {
        //Speed acceleration damping
        Vector2 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
        currentSpeed = Mathf.MoveTowards(currentSpeed, moveAxis.magnitude * playerSpeed, Time.deltaTime * playerAcceleration);
    }

    public void CheckedMove(Vector3 moveAxis)
    {

        playerCollision.CollisionCheck(moveAxis, currentSpeed * Time.deltaTime, playerCollision.WallLayer, out Vector3 finalPosition, out RaycastHit2D hit);
        transform.position = finalPosition;

    }

    public void RotateToMoveDirection(Vector3 moveAxis)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, moveAxis);

    }

}
