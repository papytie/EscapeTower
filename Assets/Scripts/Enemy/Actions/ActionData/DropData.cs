using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropData : IActionData
{
    [Header("Settings")]
    public GameObject dropItem;
    public float cooldown;
    public float duration;
    public float reactionTime;
    public Vector2 dropPositionOffset;
}
