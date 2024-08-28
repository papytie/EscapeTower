using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DropItemConfig
{
    [Header("Settings")]
    public DropItem item = null;
    public Vector2 dropPositionOffset = Vector2.zero;
    public float damage = 1f;
    public float lifespan = 5f;
    public float cooldown = 1f;
    public float minDistBetweenItems = .1f;
    public float maxItemCount = 100;

    [Header("Options")]
    public bool isEternal = false; 
    public bool isDestructible = false; 
}
