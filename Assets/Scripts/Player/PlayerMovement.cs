using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;

    PlayerInputs playerInputs;
    Animator playerAnimator;

    public void InitRef(PlayerInputs inputRef, Animator playerAnimRef)
    {
        playerInputs = inputRef;
        playerAnimator = playerAnimRef;
    }

    public void Move()
    {
        Vector3 moveAxis = playerInputs.MoveAxisInput.ReadValue<Vector2>();
        transform.position +=  playerSpeed * Time.deltaTime * moveAxis;
        playerAnimator.SetFloat(GameParams.Animation.PLAYER_RIGHTDAXIS_FLOAT, moveAxis.x);
        playerAnimator.SetFloat(GameParams.Animation.PLAYER_FORWARDAXIS_FLOAT, moveAxis.y);

        if (moveAxis != Vector3.zero)
        {
            Quaternion rotateDirection = Quaternion.LookRotation(Vector3.forward, moveAxis);
            transform.rotation = rotateDirection;
        }
    }
}
