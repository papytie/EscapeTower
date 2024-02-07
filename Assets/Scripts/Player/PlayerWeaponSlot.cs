using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSlot : MonoBehaviour
{
    public PlayerWeapon EquippedWeapon => equippedWeapon;

    [SerializeField] PlayerWeapon equippedWeapon = null;
    [SerializeField] Vector3 slotPosition;
    [SerializeField] Vector3 slotRotation;

    private void Start()
    {
        equippedWeapon.transform.SetLocalPositionAndRotation(slotPosition, Quaternion.Euler(slotRotation));
    }

    public void EquipWeapon(PlayerWeapon weapon)
    {
        if(equippedWeapon != null)
            Destroy(equippedWeapon.gameObject);

        
        equippedWeapon = Instantiate(weapon, transform);
        equippedWeapon.transform.SetLocalPositionAndRotation(slotPosition, Quaternion.Euler(slotRotation));

    }

}
