using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EyeBehaviourData : IBehaviourData
{
    public List<AttackConfig> AttacksList => attacksList;
    public List<MovementConfig> MovesList => movesList;

    protected List<MovementConfig> movesList = new();
    protected List<AttackConfig> attacksList = new();

    public MovementConfig roam;
    public MovementConfig chase;
    public AttackConfig charge;

    public void InitConfigList()
    {
        movesList.Add(new MovementConfig(MovementType.Roam));
        movesList.Add(new MovementConfig(MovementType.Chase));
        attacksList.Add(new AttackConfig(AttackType.Charge));
    }

    public void SetConfig()
    {
        foreach (AttackConfig attackConfig in attacksList)
        {
            switch (attackConfig.AttackType)
            {
                case AttackType.Charge:
                    charge = attackConfig;
                    break;

            }
        }

        foreach (MovementConfig movementConfig in movesList)
        {
            switch (movementConfig.MoveType)
            {
                case MovementType.Roam:
                    roam = movementConfig;
                    break;
                case MovementType.Chase:
                    chase = movementConfig;
                    break;
            }
        }
    }
}
