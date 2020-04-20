using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        theStateManager = GameObject.FindObjectOfType<StateManager>();
        targetPosition = this.transform.position;
      

    }
    
   
    //int ScoreA ;
   // int ScoreB;
    public StoneStorage MyStoneStorage;
    public int PlayerId;
    public Tile currentTile { get; protected set; }
    public bool scoreMe = false;
    StateManager theStateManager;
    StoneStorage theStoneStorage;
    Tile[] moveQueue;
    int moveQueueIndex;
    bool isAnimating = false;
    Vector3 targetPosition;
    Vector3 velocity = Vector3.zero;
    float smoothTime = 0.25f;
    float smoothTimeVertical = 0.01f;
    float smoothDiatance = 0.01f;
    float smoothHeight = 0f;
    PlayerStone stoneToBop;
    public Tile StartingTile { get; internal set; }

    // Update is called once per frame
    void Update()
    {

        if (isAnimating == false)
        {
            //nothing for us todo.
            return;
        }
        if (Vector3.Distance(new Vector3(this.transform.position.x, targetPosition.y, this.transform.position.z), targetPosition) < smoothDiatance)
        {
            //we've reached the target position -- do we still have moves in the queue?

            if ((moveQueue == null || moveQueueIndex == (moveQueue.Length))
                &&
                ((this.transform.position.y - smoothDiatance) > targetPosition.y)
                )
            {

                // drop down
                this.transform.position = Vector3.SmoothDamp(this.transform.position,
                new Vector3(this.transform.position.x, targetPosition.y, this.transform.position.z),
                ref velocity,
                smoothTimeVertical);

                //check for bops
                if (stoneToBop != null)
                {
                    stoneToBop.ReturnToStorage();
                    stoneToBop = null;
                }
            }


            else
            {
                //right position,right height.
                AdvanceMoveQueue();
            }
        }

        else if (this.transform.position.y < (smoothHeight - smoothDiatance))
        {
            // rise up before  move sideways.
            this.transform.position = Vector3.SmoothDamp(this.transform.position,
                new Vector3(this.transform.position.x, smoothHeight, this.transform.position.z),
                ref velocity,
                smoothTimeVertical);

        }
        else
        {
            //normal movement 
            this.transform.position = Vector3.SmoothDamp(this.transform.position,
                new Vector3(targetPosition.x, smoothHeight, targetPosition.z),
                ref velocity,
                smoothTime);

        }

    }
    void AdvanceMoveQueue()
    {
        if (moveQueue != null && moveQueueIndex < moveQueue.Length)
        {
            Tile nextTile = moveQueue[moveQueueIndex];
            if (nextTile == null)
            {
                Debug.Log("scoring Tile");
                //we are probably being scored
                
                SetNewTargetPosition(this.transform.position + Vector3.right * 10f);
            }
            else
            {
                SetNewTargetPosition(nextTile.transform.position);
                moveQueueIndex++;
            }

        }
        else
        {
            //the movement queue is empty , so we are done animating!
            this.isAnimating = false;
            
            theStateManager.AnimationsPlaying--;

            
        }
    }
    void SetNewTargetPosition(Vector3 pos)
    {
        targetPosition = pos;
        velocity = Vector3.zero;
        isAnimating = true;
    }

    public void OnMouseUp()
    {

        MoveMe();
    }

    public void MoveMe()
    {

        //is this the correct player?
        if (theStateManager.CurrentPlayerId != PlayerId)
        {
            return;
        }

        //have we rolled the dice?
        if (theStateManager.IsDoneRolling == false)
        {
            //we haven't move yet.
            return;
        }
        if (theStateManager.IsDoneClicking == true)
        {
            //we've already done a move
            return;
        }
        int spacesToMove = theStateManager.DiceTotal;
        
        //where should we end up.

        moveQueue = GetTilesAhead(spacesToMove);
        Tile FinalTile = moveQueue[moveQueue.Length - 1];

        // if the destination is legal!

        if (FinalTile == null && CanLegallyMoveto(FinalTile) == true)
        {


            return;
         

        }
        
        else
        {
            
            if (CanLegallyMoveto(FinalTile) == false)
            {
                //not allowed
                FinalTile = currentTile;
                moveQueue = null;
                return;
            }

            // kik it out.
            if (FinalTile.PlayerStone != null)
            {
                stoneToBop = FinalTile.PlayerStone;
                stoneToBop.currentTile.PlayerStone = null;
                stoneToBop.currentTile = null;

            }


        }

        this.transform.SetParent(null);

        //remove ourselves from our old tile
        if (currentTile != null)
        {
            currentTile.PlayerStone = null;

        }

        // set our current tile to the new tile.
        currentTile = FinalTile;
        
       
        if (FinalTile.IsScoringSpace == false) //scoring tiles are always empty.
        {
            FinalTile.PlayerStone = this;

        }


        moveQueueIndex = 0;


        theStateManager.IsDoneClicking = true;
        this.isAnimating = true;
        theStateManager.AnimationsPlaying++;
    }
    //return the list of tiles moves ahead
    public Tile[] GetTilesAhead(int spacesToMove)
    {
        
        Tile[] listOfTiles = new Tile[spacesToMove];
        //where should we end up.

        Tile FinalTile = currentTile;
        for (int i = 0; i < spacesToMove; i++)
        {
            if (FinalTile == null)
            {
                FinalTile = StartingTile;
            }
            else
            {
                if (FinalTile.NextTiles == null || FinalTile.NextTiles.Length == 0)
                {
                    //we are overshooting the victory  
                    break;

                }
                else if (FinalTile.NextTiles.Length > 1)
                {
                    //branch based on player id
                    FinalTile = FinalTile.NextTiles[PlayerId];
                }
                else
                {
                    FinalTile = FinalTile.NextTiles[0];
                }

            }
            listOfTiles[i] = FinalTile;
        }
        return listOfTiles;
    }
    

    public Tile GetTileAhead()
    {
        return GetTileAhead(theStateManager.DiceTotal);
    }

    public Tile GetTileAhead(int spacesToMove)
    {
        //Debug.Log(spacesToMove);
        Tile[] tiles = GetTilesAhead(spacesToMove);

        if (tiles == null)
        {
            //we aren't moving at all
            return currentTile;
        }
        return tiles[tiles.Length - 1];
    }
    public bool CanLegallyMoveAhead(int spacesToMove)
    {
        Tile theTile = GetTileAhead(spacesToMove);
        return CanLegallyMoveto(theTile);
    }
    bool CanLegallyMoveto(Tile destinationTile)
    {
        // Debug.Log("CanLegallyMoveto: " + destinationTile);
        if (destinationTile == null)
        {
            return false;

           
        }

        //is the tile empty?
        if (destinationTile.PlayerStone == null)
        {
            return true;
        }

        if (destinationTile.PlayerStone.PlayerId == this.PlayerId)
        {
            //we can't land on our own stone.
            return false;
        }
        
        return true;
    }
    public void ReturnToStorage()
    {

        Debug.Log("ReturnToStorage");
       

        this.isAnimating = true;
        theStateManager.AnimationsPlaying++;

        moveQueue = null;

        //save our current position
        Vector3 savePosition = this.transform.position;
        MyStoneStorage.AddStoneToStorage(this.gameObject);

        //set our new position to the animation target
        SetNewTargetPosition(this.transform.position);

        //restore our saved position
        this.transform.position = savePosition;
    }
}
