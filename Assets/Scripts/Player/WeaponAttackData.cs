using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAttackData", menuName = "GameData/WeaponAttackData", order = 1)]

public class WeaponAttackData : ScriptableObject
{
    [Header("Attack Data"), Space] 
    public AttackData attackData;
    
    //[Header(" Data"), Space]
    //public AttackData attackData;

}
