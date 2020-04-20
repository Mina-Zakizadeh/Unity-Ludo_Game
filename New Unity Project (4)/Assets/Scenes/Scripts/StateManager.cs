using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    // Start is called before the first frame update


    void Start()
    {
        theTile = GameObject.FindObjectOfType<Tile>();
        PlayerAIs = new AIPlayer[NumberOfPlayers];

        PlayerAIs[0] = null;  //is a human player
        //PlayerAIs[0] = new AIPlayer();

        PlayerAIs[1] = new AIPlayer_UtilityAI();

    }
    bool turn = true;
    int[] NumberOfScore= { 0,0};
    public int NumberOfPlayers = 2;
    public int CurrentPlayerId = 0;
    Tile theTile;
    AIPlayer[] PlayerAIs;
    PlayerStone thePlayerStone;
    public bool IsDoneRolling = false;
    public bool IsDoneClicking = false;
    public int AnimationsPlaying = 0;
    public int DiceTotal;
    public GameObject NoLegalMovesPopup;
    public GameObject YouWinpopup;
    public void NewTurn()
    {
        
        IsDoneRolling = false;
        IsDoneClicking = false;
        
        if (DiceTotal < 6)
        {
            CurrentPlayerId = (CurrentPlayerId + 1) % NumberOfPlayers;
        }

    }

    public void RollAgain()
    {


        Debug.Log("RollAgain");

        IsDoneRolling = false;
        IsDoneClicking = false;

    }

    // Update is called once per frame
    void Update()
    {
        //is the turn done?
        if (AnimationsPlaying == 0 && IsDoneClicking && IsDoneRolling)
        {

            Debug.Log("turn is done!");
            PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
            foreach (PlayerStone ps in pss)
            {
                if (ps.PlayerId == CurrentPlayerId && ps.scoreMe == false)
                {

                    if (ps.currentTile != null && ps.currentTile.IsScoringSpace)
                    {
                        NumberOfScore[CurrentPlayerId]++;
                        ps.scoreMe = true;
                        if (NumberOfScore[CurrentPlayerId] == 4)
                        {
                            StartCoroutine(YouWinCoroutine());
                            turn = false;
                        }
                        Debug.Log(NumberOfScore[CurrentPlayerId]);
                    }
                }
            }
            if (turn)
            {
                NewTurn();
                return;
            }
           
        }
        if (PlayerAIs[CurrentPlayerId] != null)
        {
            PlayerAIs[CurrentPlayerId].DoAI();
        }
        
    }
    public void CheckLegalMoves()
    {
        

        //loop through all of a player's stones
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
        bool hasLegalMove = false;
        foreach (PlayerStone ps in pss)
        {
            if (ps.PlayerId == CurrentPlayerId)
            {
                if (ps.CanLegallyMoveAhead(DiceTotal))
                {
                    hasLegalMove = true;
                   
                }
            }
        }
        if (hasLegalMove == false)
        {
            StartCoroutine(NoLegalMoveCoroutine());
            return;
        }
       
    }
    IEnumerator NoLegalMoveCoroutine()
    {
        //display message
        NoLegalMovesPopup.SetActive(true);


        //wait 1 sec
        yield return new WaitForSeconds(1f);
        NoLegalMovesPopup.SetActive(false);

        NewTurn();
    }
    public IEnumerator YouWinCoroutine()
    {
        //display message
        YouWinpopup.SetActive(true);


        //wait 1 sec
        yield return new WaitForSeconds(1f);
       // YouWinpopup.SetActive(false);

        //NewTurn();
    }
}
