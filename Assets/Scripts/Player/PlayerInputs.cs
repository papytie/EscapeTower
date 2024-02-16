using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public InputAction MouseAttackButtonInput => controls.Character.MouseAttack;
    public InputAction GamepadAttackButtonInput => controls.Character.GamepadAttack;
    public InputAction AttackAxisInput => controls.Character.AttackOrientation;
    public InputAction MoveAxisInput => controls.Character.Move;
    public InputAction DashButtonInput => controls.Character.Dash;
    public InputAction MousePositionAxisInput => controls.Character.MousePos;

    GameInputSettings controls = null;

    private void Awake()
    {
        controls = new GameInputSettings();
        controls.Character.Enable();
    }

}
