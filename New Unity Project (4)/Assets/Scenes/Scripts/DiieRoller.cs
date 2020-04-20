using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiieRoller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DiceValues = new int[6];
        theStateManager = GameObject.FindObjectOfType<StateManager>();
    }
    Tile theTiles;
    StateManager theStateManager;
    public int[] DiceValues;

    
    // Update is called once per frame
    void Update()
    {

    }



    public void RollTheDice()
    {
        if (theStateManager.IsDoneRolling == true)
        {
            //we've already rolled this turn.
            return;
        }
        
        theStateManager.DiceTotal = 0;
       
        theStateManager.DiceTotal = Random.Range(1, 7);
        theStateManager.IsDoneRolling = true;
        theStateManager.CheckLegalMoves();

        //Debug.Log("Rolled" +DiceTotal );
    }
}
