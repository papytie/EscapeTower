using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EscapeTower/GameSettings", order = 100)]
public class GameSettings : ScriptableObject {

    #region Singleton
    private static GameSettings instance;

    public static GameSettings Instance {
        get {
            if(instance == null) {
                instance = Resources.Load<GameSettings>(GameParams.Path.GAME_SETTINGS_PATH);
                if(instance == null) {
                    Debug.LogError("Error - No GameSettings in project");
                }
            }
            return instance;
        }
    }
    #endregion

    [Header("Pickup Color")]
    public Color bonusColor;
    public Color weaponColor;
    public Color consumableColor;
}
