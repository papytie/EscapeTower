using System;
using System.Reflection;
using UnityEngine;

[ExecuteAlways]
public class SpriteSecondaryTextureReflection : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public int secondaryTextureNum;
    public Texture2D secondaryTex;

    private void OnEnable()
    {
        secondaryTex = (Texture2D)_spriteRenderer.sprite.GetSecondaryTexture(secondaryTextureNum);

        if (secondaryTex != null)
        {
            Debug.Log($"Got Secondary Tex: {secondaryTex.name}");
        }
    }
}

public static class SpriteUtils
{
    private delegate Texture2D GetSecondaryTextureDelegate(Sprite sprite, int index);

    private static readonly GetSecondaryTextureDelegate GetSecondaryTextureCached =
        (GetSecondaryTextureDelegate)Delegate.CreateDelegate(
            typeof(GetSecondaryTextureDelegate),
            typeof(Sprite).GetMethod("GetSecondaryTexture", BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new Exception("Unity has changed/removed the internal method Sprite.GetSecondaryTexture"));

    public static Texture GetSecondaryTexture(this Sprite sprite, int index) => GetSecondaryTextureCached(sprite, index);
}