using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject {

    public static GameSettings Instance {
        get {
            if(!SRResources.GameSettings.IsLoaded)
                SRResources.GameSettings.Load();
            return SRResources.GameSettings;
        }
    }

    [Header("Pickup Color")]
    public Color bonusColor;
    public Color weaponColor;
    public Color consumableColor;
}
