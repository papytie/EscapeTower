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

    public void InitRef(PlayerStats statsRef)
    {
        stats = statsRef;
    }
   
    private void Start()
    {
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

        equippedWeapon = Instantiate(weapon, transform);
        equippedWeapon.InitRef(this, stats);
        equippedWeapon.transform.SetPositionAndRotation(slotTransform.position, Quaternion.Euler(slotTransform.rotation.eulerAngles));

    }

}
