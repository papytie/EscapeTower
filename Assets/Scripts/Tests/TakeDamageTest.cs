using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TakeDamageTest : MonoBehaviour
{
    [SerializeField] PlayerLifeSystem lifeSystem;
    [SerializeField] int damage = 1;

    private void Update()
    {
        if(Keyboard.current.pKey.wasPressedThisFrame)
            lifeSystem.TakeDamage(damage);
    }
}
