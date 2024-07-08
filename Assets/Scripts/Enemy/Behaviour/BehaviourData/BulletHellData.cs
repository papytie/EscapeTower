using System;
using System.Collections.Generic;

[Serializable]
public class BulletHellData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;
    public List<ActionConfig> actions;

    public BulletHellData()
    {
        actions = new List<ActionConfig>
        {
            { new ActionConfig(ActionType.WaitMove, ActionID.WAIT) },
            { new ActionConfig(ActionType.WaitMove, ActionID.ATTACKSELECTION) },
            { new ActionConfig(ActionType.RoamMove, ActionID.ROAM) },
            { new ActionConfig(ActionType.StayAtRangeMove, ActionID.STAYATRANGE) },
            { new ActionConfig(ActionType.RangedAttack, ActionID.SINGLE_SHOT) },
            { new ActionConfig(ActionType.RangedAttack, ActionID.SUPER_SHOT) },
            { new ActionConfig(ActionType.RangedAttack, ActionID.LASER_SHOT) },
            { new ActionConfig(ActionType.RangedAttack, ActionID.SPREAD_SHOT) },
            { new ActionConfig(ActionType.RangedAttack, ActionID.MULTI_SHOT) },
            { new ActionConfig(ActionType.RangedAttack, ActionID.LASER_ULTIMATE) },
            { new ActionConfig(ActionType.RangedAttack, ActionID.NOVA_ULTIMATE) },
            { new ActionConfig(ActionType.RangedAttack, ActionID.GALAXY_ULTIMATE) },
            { new ActionConfig(ActionType.TakeDamageReaction, ReactionID.TAKEDMG) },
            { new ActionConfig(ActionType.DieReaction, ReactionID.DIE) }
        };
    }

    public enum ShotType
    {
        SINGLE = 0, SUPER = 1, LASER = 2, SPREAD = 3, MULTI = 4,
    }

    public enum UltimateType
    {
        LASER = 0, NOVA = 1, GALAXY = 2, 
    }

    public class ActionID
    {
        public static string WAIT = "Wait";
        public static string ATTACKSELECTION = "AttackSelection";
        public static string ROAM = "Roam";
        public static string STAYATRANGE = "StayAtRange";
        public static string SINGLE_SHOT = "SingleShot";
        public static string SUPER_SHOT = "SuperShot";
        public static string LASER_SHOT = "LaserShot";
        public static string SPREAD_SHOT = "SpreadShot";
        public static string MULTI_SHOT = "MultiShot";
        public static string LASER_ULTIMATE = "LaserUltimate";
        public static string NOVA_ULTIMATE = "NovaUltimate";
        public static string GALAXY_ULTIMATE = "GalaxyUltimate";
    }
}


