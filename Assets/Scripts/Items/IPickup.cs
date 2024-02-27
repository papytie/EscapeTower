using UnityEngine;

interface IPickup
{
    public PickableType Type { get; }
    public Sprite Sprite { get; }
}
