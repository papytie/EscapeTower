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

    public bool IsInputScheme(InputAction action, InputSchemeEnum schemeEnum) {
        if(action.activeControl == null)
            return false;

        int index = action.GetBindingIndexForControl(action.activeControl);
        string[] actionGroups = action.bindings[index].groups.Split(";");
        InputControlScheme scheme = GetControlScheme(schemeEnum);

        foreach(string group in actionGroups) {
            if(group == scheme.bindingGroup)
                return true;
        }
        return false;
    }

    private InputControlScheme GetControlScheme(InputSchemeEnum schemeEnum) {
        return schemeEnum switch {
            InputSchemeEnum.Gamepad => controls.GamepadScheme,
            InputSchemeEnum.KeyboardMouse => controls.KeyboardMouseScheme,
            _ => controls.KeyboardMouseScheme,
        };
    }
}

public enum InputSchemeEnum {
    Gamepad = 0,
    KeyboardMouse = 1
}
