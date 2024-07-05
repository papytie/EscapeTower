using System;
using UnityEngine;

[Serializable]
public class ActionConfig
{
    public ActionType ActionType => actionType;

    public ActionConfig(ActionType type, string ID)
    {
        actionType = type;
        ActionID = ID;
    }

    public string ActionID;
    protected ActionType actionType;
    [SerializeReference] public IActionData data;
}