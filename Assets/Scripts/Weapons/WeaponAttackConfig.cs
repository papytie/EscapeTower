using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAttackData", menuName = "GameData/WeaponAttackData", order = 1)]

public class WeaponAttackConfig : ScriptableObject
{
    [Header("Attack Data"), Space] 
    public WeaponAttackData attackData;
    
}
