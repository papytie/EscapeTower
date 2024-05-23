using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSlot : MonoBehaviour
{
    public WeaponController EquippedWeapon => equippedWeapon;
    public Transform SlotTransform => slotTransform;

    [SerializeField] WeaponController equippedWeapon = null;
    [SerializeField] Transform slotTransform;

    PlayerStats stats;
    Vector2 slotStartPos;

    public void InitRef(PlayerStats statsRef)
    {
        stats = statsRef;
        slotStartPos = slotTransform.localPosition;

        equippedWeapon = GetComponentInChildren<WeaponController>();
        if (equippedWeapon == null)
        {
            Debug.LogWarning("Player has no weapon");
            return;
        }

        equippedWeapon.transform.SetPositionAndRotation(slotTransform.position, Quaternion.Euler(slotTransform.rotation.eulerAngles));
        equippedWeapon.InitRef(this, stats);
    }

    public void EquipWeapon(WeaponController weapon)
    {
        if(equippedWeapon != null)
            Destroy(equippedWeapon.gameObject);

        equippedWeapon = Instantiate(weapon, slotTransform);
        equippedWeapon.InitRef(this, stats);
        equippedWeapon.transform.SetPositionAndRotation(slotTransform.position, Quaternion.Euler(slotTransform.rotation.eulerAngles));

    }

    public void RotateSlot(Vector2 direction)
    {
        Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, direction);
        slotTransform.SetLocalPositionAndRotation(currentRotation * slotStartPos, currentRotation);
    }

}
