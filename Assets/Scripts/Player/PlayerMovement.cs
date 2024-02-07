using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    PlayerInputs playerInputs;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void InitRef(PlayerInputs inputRef)
    {
        playerInputs = inputRef;
    }

    public void Move()
    {
        Vector3 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
        transform.position += moveAxis * playerSpeed * Time.deltaTime;

        if (moveAxis != Vector3.zero)
        {
            Quaternion rotateDirection = Quaternion.LookRotation(Vector3.forward, moveAxis);
            transform.rotation = rotateDirection;
        }
    }
}
