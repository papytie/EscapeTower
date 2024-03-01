using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    Vector2 startPosition = Vector2.zero;
    Vector2 endPosition = Vector2.zero;
    PlayerWeapon weapon;
    float currentTime = 0;

    public void Init(PlayerWeapon weaponRef, Vector2 endPos)
    {
        weapon = weaponRef;
        startPosition = weapon.transform.position;
        endPosition = endPos;
    }

    private void Update()
    {
        ProjectileMovement(startPosition, endPosition);
    }

    void ProjectileMovement(Vector2 startPos, Vector2 endPos)
    {
        currentTime += Time.deltaTime;
        transform.position = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(currentTime / weapon.HitboxDuration));
        if (currentTime >= weapon.HitboxDuration)
        {
            Destroy(gameObject);
        }
    }
}
