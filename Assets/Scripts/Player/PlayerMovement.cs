using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed => moveSpeed;

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float acceleration = 50f;
    float currentSpeed = 0;

    PlayerInputs inputs;
    CollisionCheckerComponent collision;
    PlayerStats stats;

    public void InitRef(PlayerInputs inputRef, CollisionCheckerComponent collisionRef, PlayerStats statsRef)
    {
        inputs = inputRef;
        collision = collisionRef;
        stats = statsRef;
    }

    private void Update()
    {
        //Speed acceleration damping
        Vector2 moveAxis = inputs.MoveAxisInput.ReadValue<Vector2>();

        currentSpeed = Mathf.MoveTowards(currentSpeed, moveAxis.magnitude * stats.GetModifiedMainStat(MainStat.MoveSpeed), Time.deltaTime * acceleration);
    }

    public void CheckedMove(Vector3 moveAxis)
    {

        collision.MoveToCollisionCheck(moveAxis, currentSpeed * Time.deltaTime, collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
        transform.position = finalPosition;

    }

    public void RotateToMoveDirection(Vector3 moveAxis)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, moveAxis);

    }

}
