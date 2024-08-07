using System;

[Serializable]
public class WaitData : IActionData
{
    public float minTime = 1f;
    public float maxTime = 3f;
}
