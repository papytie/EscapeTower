using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchWeaponTest : MonoBehaviour
{
    [SerializeField] PlayerWeaponSlot weaponSlot;
    [SerializeField] PlayerWeapon weaponOne;
    [SerializeField] PlayerWeapon weaponTwo;
    bool weaponSwitch = false;


    void Update()
    {
        if(Keyboard.current.rKey.wasPressedThisFrame)
            SwapWeapon();
    }

    public void SwapWeapon()
    {
        if (weaponSwitch)
        {
            weaponSlot.EquipWeapon(weaponOne);
            weaponSwitch = false;
        }

        else
        {
            weaponSlot.EquipWeapon(weaponTwo);
            weaponSwitch = true;
        }
    }

}
