using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float playerAcceleration = 50f;
    float currentSpeed = 0;

    PlayerInputs inputs;
    PlayerCollision collision;

    public void InitRef(PlayerInputs inputRef, PlayerCollision collisionRef)
    {
        inputs = inputRef;
        collision = collisionRef;
    }

    private void Update()
    {
        //Speed acceleration damping
        Vector2 moveAxis = inputs.MoveAxisInput.ReadValue<Vector2>();
        currentSpeed = Mathf.MoveTowards(currentSpeed, moveAxis.magnitude * playerSpeed, Time.deltaTime * playerAcceleration);
    }

    public void CheckedMove(Vector3 moveAxis)
    {

        collision.MoveCollisionCheck(moveAxis, currentSpeed * Time.deltaTime, collision.WallLayer, out Vector3 finalPosition, out RaycastHit2D hit);
        transform.position = finalPosition;

    }

    public void RotateToMoveDirection(Vector3 moveAxis)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, moveAxis);

    }

}
