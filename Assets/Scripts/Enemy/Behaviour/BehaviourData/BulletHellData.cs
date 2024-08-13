using System;
using System.Collections.Generic;

[Serializable]
public class BulletHellData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;
    List<ActionConfig> actions;

    public ActionConfig wait;
    public ActionConfig attackSelection;
    public ActionConfig roam;
    public ActionConfig singleShot;
    public ActionConfig superShot;
    public ActionConfig spreadShot;
    public ActionConfig multiShot;
    public ActionConfig beamUltimate;
    public ActionConfig novaUltimate;
    public ActionConfig galaxyUltimate;
    public ActionConfig die;

    public void InitActionsList()
    {
        actions = new List<ActionConfig>
        {
            wait,
            attackSelection,
            roam,
            singleShot,
            superShot,
            spreadShot,
            multiShot,
            beamUltimate,
            novaUltimate,
            galaxyUltimate,
            die
        };
    }

    public enum ShotType
    {
        SINGLE = 0, SUPER = 1, SPREAD = 2, MULTI = 3,
    }

    public enum UltimateType
    {
        BEAM = 0, NOVA = 1, GALAXY = 2, 
    }
}