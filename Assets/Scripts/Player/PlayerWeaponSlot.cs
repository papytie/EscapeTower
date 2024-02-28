using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSlot : MonoBehaviour
{
    public PlayerWeapon EquippedWeapon => equippedWeapon;
    public Transform SlotTransform => slotTransform;

    [SerializeField] PlayerWeapon equippedWeapon = null;
    [SerializeField] Transform slotTransform;

    private void Start()
    {
        equippedWeapon.transform.SetLocalPositionAndRotation(slotTransform.position, Quaternion.Euler(slotTransform.rotation.eulerAngles));
        equippedWeapon.InitRef(this);
    }

    public void EquipWeapon(PlayerWeapon weapon)
    {
        if(equippedWeapon != null)
            Destroy(equippedWeapon.gameObject);

        equippedWeapon = Instantiate(weapon, transform);
        equippedWeapon.InitRef(this);
        equippedWeapon.transform.SetPositionAndRotation(slotTransform.position, Quaternion.Euler(slotTransform.rotation.eulerAngles));

    }

}
