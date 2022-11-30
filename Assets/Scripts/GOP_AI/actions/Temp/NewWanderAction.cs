using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWanderAction : Action
{
// ███    █▄  ███▄▄▄▄      ▄████████  ▄█  ███▄▄▄▄    ▄█     ▄████████    ▄█    █▄       ▄████████ ████████▄  
// ███    ███ ███▀▀▀██▄   ███    ███ ███  ███▀▀▀██▄ ███    ███    ███   ███    ███     ███    ███ ███   ▀███ 
// ███    ███ ███   ███   ███    █▀  ███▌ ███   ███ ███▌   ███    █▀    ███    ███     ███    █▀  ███    ███ 
// ███    ███ ███   ███  ▄███▄▄▄     ███▌ ███   ███ ███▌   ███         ▄███▄▄▄▄███▄▄  ▄███▄▄▄     ███    ███ 
// ███    ███ ███   ███ ▀▀███▀▀▀     ███▌ ███   ███ ███▌ ▀███████████ ▀▀███▀▀▀▀███▀  ▀▀███▀▀▀     ███    ███ 
// ███    ███ ███   ███   ███        ███  ███   ███ ███           ███   ███    ███     ███    █▄  ███    ███ 
// ███    ███ ███   ███   ███        ███  ███   ███ ███     ▄█    ███   ███    ███     ███    ███ ███   ▄███ 
// ████████▀   ▀█   █▀    ███        █▀    ▀█   █▀  █▀    ▄████████▀    ███    █▀      ██████████ ████████▀  
                                                

    /*
        This Action class will attempt to move the agent into a random direction
        Note that this random direction is actually weighted. The partition system
        is kind enough to have a real-time score of each partition zone. If the agent 
        find itself hungry or thirsty with no access to these, it can get a helping 
        hand from the partition system and get a hint towards which partitions to move towards

        This action is broken down into the following steps:
        1) Is the agent eligible to get a hint from the partition system? If so, get a some scores
        and move towards the best one (TODO)
        2) if not, move in a random direction
        3) continue until either hitting an obstacle or ran out of time
        4) return control to the agent
    */

    #region Fields
    private Vector3 direction;
    private float distance;
    private Vector3 endPoint;
    #endregion
    #region Inherrited methods
    public override bool isActionPossible(GOPAgent _agent){return true;}
    public override float ActionScore()
    {
        return 0;
    }
    public override void PerformAction()
    {

    }
    public override void UpdateAction()
    {
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
    }
    #endregion
    #region Action states
    public void MoveInDirection()
    {

    }
    #endregion
    #region Misc
    //TODO
    private bool EligibleForHint()
    {
        return false;
    }
    #endregion
}
