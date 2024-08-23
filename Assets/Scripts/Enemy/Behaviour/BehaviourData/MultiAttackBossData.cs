using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MultiAttackBossData : IBehaviourData
{
    //Constructor to create Additive Behaviour Data from the factory
    public MultiAttackBossData(IBehaviourData data)
    {
        additiveBehaviourData = data;
    }

    public List<ActionConfig> Actions => actions;
    List<ActionConfig> actions;

    public ActionConfig wait;
    public ActionConfig attackSelection;
    public ActionConfig roam;
    public ActionConfig die;

    public List<ActionConfig> shots;
    public List<ActionConfig> ultimates;

    [SerializeReference] public IBehaviourData additiveBehaviourData;

    public void InitActionsList()
    {
        actions = new List<ActionConfig>
        {
            wait,
            attackSelection,
            roam,
            die
        };

        foreach (var shot in shots) { actions.Add(shot); }

        foreach (var ultimate in ultimates) { actions.Add(ultimate); }
    }
}