using System;
using UnityEngine;

[Serializable]
public class ActionConfig
{
    public ActionStateType StateType => type;

    public ActionConfig(ActionStateType moveType)
    {
        type = moveType;
        name = moveType.ToString();
    }

    public string name;
    protected ActionStateType type;
    [SerializeReference] public IActionData data;
}