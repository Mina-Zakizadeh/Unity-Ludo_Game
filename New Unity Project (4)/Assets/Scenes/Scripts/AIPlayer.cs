using UnityEngine;

using System.Collections.Generic;


public class AIPlayer
{
    public AIPlayer()
    {
        stateManager = GameObject.FindObjectOfType<StateManager>();
    }
    StateManager stateManager;
    virtual public void DoAI()
    {
        
        if (stateManager.IsDoneRolling == false)
        {
            DoRoll();
            return;
        }
        if (stateManager.IsDoneClicking == false)
        {
            // pick a stone to move
            DoClick();
            return;
        }


    }

    virtual protected void DoRoll()
    {
        GameObject.FindObjectOfType<DiieRoller>().RollTheDice();
    }
    virtual protected void DoClick()
    {
        //pick a stone to move.



        PlayerStone[] legalStones = GetLegalMoves();

        if (legalStones == null || legalStones.Length == 0)
        {
            //we have no legal moves.
            return;
        }
        // picks a legal move at  random.
        PlayerStone pickStone = PickStoneToMove(legalStones);

        pickStone.MoveMe();
    }
    virtual protected PlayerStone PickStoneToMove(PlayerStone[] legalStones)
    {
        return legalStones[Random.Range(0, legalStones.Length)];
    }


    ///<summary>
    ///return a list of stones that can legally moved
    ///</summary>
    protected PlayerStone[] GetLegalMoves()
    {
        List<PlayerStone> legalStones = new List<PlayerStone>();

        

        //loop through all of a player's stones
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
        foreach (PlayerStone ps in pss)
        {
            if (ps.PlayerId == stateManager.CurrentPlayerId)
            {
                if (ps.CanLegallyMoveAhead(stateManager.DiceTotal))
                {
                    legalStones.Add(ps);
                }
            }
        }
        return legalStones.ToArray();
    }
}

