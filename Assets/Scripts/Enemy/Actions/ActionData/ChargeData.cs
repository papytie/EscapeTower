using System;

[Serializable]
public class ChargeData : IActionData
{
    public int priority = 0;
    public float cooldown = .5f;
    public float lag = .2f;
    public float delay = 0;
    public float duration = .1f;
    public HitboxSettingsData hitboxSettings;
}