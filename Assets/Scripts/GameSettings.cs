using UnityEngine;
using EscapeTower.GameSettings;

public class GameSettings : ScriptableObject {

    [Header("Sound Settings"), Space]
    public MusicSettings Music;

    [Header("Pickup Settings"), Space]
    public PickupSettings Pickup;
}
