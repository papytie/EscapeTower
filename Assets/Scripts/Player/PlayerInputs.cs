using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public InputAction AttackButtonInput => attackButtonInput;
    public InputAction AttackAxisInput => attackAxisInput;
    public InputAction MoveAxisInput => moveAxisInput;
    public InputAction DashButtonInput => dashButtonInput;
    public InputAction MousePositionAxisInput => mousePositionAxisInput;

    [SerializeField] PlayerControls controls = null;

    InputAction attackButtonInput;
    InputAction attackAxisInput;
    InputAction moveAxisInput;
    InputAction dashButtonInput;
    InputAction mousePositionAxisInput;
    
    private void Awake()
    {
        controls = new PlayerControls();
    }

    void Start()
    {
        InitInputs();
    }

    private void InitInputs()
    {
        attackButtonInput = controls.Character.Attack;
        attackButtonInput.Enable();
        attackAxisInput = controls.Character.AttackOrientation;
        attackAxisInput.Enable();
        moveAxisInput = controls.Character.Move;
        moveAxisInput.Enable();
        dashButtonInput = controls.Character.Dash;
        dashButtonInput.Enable();
        mousePositionAxisInput = controls.Character.MousePos;
        mousePositionAxisInput.Enable();
    }

}
