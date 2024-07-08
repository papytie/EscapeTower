using System;
using UnityEngine;

public class BulletHell : MonoBehaviour, IBehaviour
{
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;

    public string currentShot = null;
    public string currentUltimate = null;

    bool UltimateReady => fsm.GetState(BulletHellData.ActionID.GALAXY_ULTIMATE).Action.IsAvailable && fsm.GetState(BulletHellData.ActionID.LASER_ULTIMATE).Action.IsAvailable && fsm.GetState(BulletHellData.ActionID.NOVA_ULTIMATE).Action.IsAvailable;

    public void Init(EnemyController enemyController)
    {
        //Get EnemyController ref
        controller = enemyController;

        //Init Reaction state
        Init_TakeDamageReaction();
        Init_DieReaction();

        //Customize each Init state
        Init_WaitState();
        Init_AttackSelectionState();
        Init_RoamState();
        Init_StayAtRangeState();
        Init_SingleShotState();
        Init_SuperShotState();
        Init_MultiShotState();
        Init_SpreadShotState();
        Init_LaserShotState();
        Init_NovaUltimateState();
        Init_LaserUltimateState();
        Init_GalaxyUltimateState();

        //Set this Behaviour starting default state
        fsm.SetState(BulletHellData.ActionID.WAIT);
    }

    void Init_WaitState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.WAIT);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired && state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.ATTACKSELECTION);
            
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.ROAM);

        };
    }

    void Init_AttackSelectionState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.ATTACKSELECTION);

        state.OnStateEnter += () => 
        {
            /* First Method called when enter State */
            currentShot = RandomShot();
            currentUltimate = RandomUltimate();
        };

        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
            {
                if(UltimateReady)
                    fsm.SetState(currentUltimate);
                
                else if (fsm.GetState(currentShot).Action.IsAvailable)
                    fsm.SetState(currentShot);
            }
            
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.ROAM);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
                fsm.SetState(BulletHellData.ActionID.STAYATRANGE);

            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);

        };

    }

    void Init_StayAtRangeState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.STAYATRANGE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (!controller.TargetAcquired)
                fsm.SetState(BulletHellData.ActionID.WAIT);

            if (controller.TargetAcquired)
                fsm.SetState(BulletHellData.ActionID.ATTACKSELECTION);
        };
    }

    void Init_SingleShotState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.SINGLE_SHOT);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    void Init_SuperShotState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.SUPER_SHOT);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    void Init_MultiShotState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.MULTI_SHOT);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    void Init_LaserShotState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.LASER_SHOT);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    void Init_SpreadShotState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.SPREAD_SHOT);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    void Init_LaserUltimateState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.LASER_ULTIMATE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    void Init_NovaUltimateState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.NOVA_ULTIMATE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    void Init_GalaxyUltimateState()
    {
        NPCState state = fsm.GetState(BulletHellData.ActionID.GALAXY_ULTIMATE);

        state.OnStateEnter += () => 
        {
            /* First Method called when enter State */ 
        };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    //-------------------------------\---/-------------------------------|
    //----------------------------|REACTIONS|----------------------------|
    //-------------------------------/---\-------------------------------|

    void Init_TakeDamageReaction()
    {
        NPCState state = fsm.GetState(ReactionID.TAKEDMG);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(BulletHellData.ActionID.WAIT);
        };
    }

    void Init_DieReaction()
    {
        NPCState state = fsm.GetState(ReactionID.DIE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
            {
                controller.LootSystem.RollLoot();
                Destroy(gameObject);
            }
        };
    }

    //------------------------------\---/--------------------------------|
    //----------------------------|HELPERS|------------------------------|
    //------------------------------/---\--------------------------------|

    string RandomShot()
    {
        int shotIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(BulletHellData.ShotType)).Length+1);

        return shotIndex switch
        {
            1 => BulletHellData.ActionID.SUPER_SHOT,
            2 => BulletHellData.ActionID.MULTI_SHOT,
            3 => BulletHellData.ActionID.SPREAD_SHOT,
            4 => BulletHellData.ActionID.LASER_SHOT,
            _ => BulletHellData.ActionID.SINGLE_SHOT,
        };
    }

    string RandomUltimate()
    {
        int ultimateIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(BulletHellData.UltimateType)).Length+1);

        return ultimateIndex switch
        {
            1 => BulletHellData.ActionID.NOVA_ULTIMATE,
            2 => BulletHellData.ActionID.GALAXY_ULTIMATE,
            _ => BulletHellData.ActionID.LASER_ULTIMATE,
        };
    }
}
