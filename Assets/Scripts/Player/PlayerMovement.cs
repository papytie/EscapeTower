using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed => moveSpeed;
    public float CurrentSpeed => currentSpeed;

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float acceleration = 50f;
    float currentSpeed = 0;

    PlayerController controller;

    public void InitRef(PlayerController playerController)
    {
        controller = playerController;
    }

    private void Update()
    {
        //Speed acceleration damping
        Vector2 moveAxis = controller.Inputs.MoveAxisInput.ReadValue<Vector2>();

        currentSpeed = Mathf.MoveTowards(currentSpeed, moveAxis.magnitude * controller.Stats.GetModifiedMainStat(MainStat.MoveSpeed), Time.deltaTime * acceleration);
    }

    public void CheckedMove(Vector3 moveAxis)
    {
        controller.Collision.MoveToCollisionCheck(moveAxis, currentSpeed * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
        transform.position = finalPosition;
        //Debug.Log($"{Time.frameCount} - Input Movement : " + currentSpeed);
    }

    public void RotateToMoveDirection(Vector3 moveAxis)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, moveAxis);

    }

}
