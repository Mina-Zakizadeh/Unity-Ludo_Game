using UnityEngine;

using System.Collections.Generic;


    public class BasicAI 
{
        public BasicAI()
        {
            stateManager = GameObject.FindObjectOfType<StateManager>();
        }
        StateManager stateManager;
        virtual public void DoAI()
        {
            //do the thing for the current stage we're in
            if (stateManager.IsDoneRolling == false)
            {
                //we need to roll the dice!
                DoRoll();
                return;
            }
            if (stateManager.IsDoneClicking == false)
            {
                //we have a die roll, but we need to pick a stone to move
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
            //pick a stone to move, then "click" it.


            PlayerStone[] legalStones = GetLegalMoves();

            if (legalStones == null || legalStones.Length == 0)
            {
                //we have no legal moves. how did we get here?
                //we might still be in a delayed coroutine somewhere. let's no freak out.
                return;
            }
            //basicAi simply picks a legal move at  random.
            PlayerStone pickStone = legalStones[Random.Range(0, legalStones.Length)];

            pickStone.MoveMe();
        }


        ///<summary>
        ///return a list of stones that can legally moved
        ///</summary>
        protected PlayerStone[] GetLegalMoves()
        {
            List<PlayerStone> legalStones = new List<PlayerStone>();

            //if we rolled a zero,then we clearly have no legal moves
            if (stateManager.DiceTotal == 0)
            {
                return legalStones.ToArray();
            }

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



