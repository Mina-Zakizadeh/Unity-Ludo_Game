using UnityEngine;
using System.Collections.Generic;

public class AIPlayer_UtilityAI : AIPlayer
{
    Dictionary<Tile, float> tileDanger;

    override protected PlayerStone PickStoneToMove(PlayerStone[] legalStones)
    {
        //Debug.Log("AIPlayer_UtilityAI");


        if (legalStones == null || legalStones.Length == 0)
        {
            Debug.LogError("Why are we being asked to pick from no stones?");
            return null;
        }

        CalcTileDanger(legalStones[0].PlayerId);

        // For each stone,how good it would be to pick it
        PlayerStone bestStone = null;
        float fitness = -Mathf.Infinity;

        foreach (PlayerStone ps in legalStones)
        {
            float g = GetStonefitness(ps, ps.currentTile, ps.GetTileAhead());
            if (bestStone == null || g > fitness)
            {
                bestStone = ps;
                fitness = g;
            }
        }

        Debug.Log("Choosen Stone fitness: " + fitness);
        return bestStone;
    }

    virtual protected void CalcTileDanger(int myPlayerId)
    {
        tileDanger = new Dictionary<Tile, float>();

        Tile[] tiles = GameObject.FindObjectsOfType<Tile>();

        foreach (Tile t in tiles)
        {
            tileDanger[t] = 0;
        }


        PlayerStone[] allStones = GameObject.FindObjectsOfType<PlayerStone>();

        foreach (PlayerStone stone in allStones)
        {
            if (stone.PlayerId == myPlayerId)
                continue;

            // This is an enemy stone

            for (int i = 1; i <= 6; i++)
            {
                Tile t = stone.GetTileAhead(i);

                if (t == null)
                {
                    // invalid
                    break;
                }

                if (t.IsScoringSpace || t.IsSideLine)
                {
                    // This tile is not a danger 
                    continue;
                }

                if (i == 3)
                {
                    // so most dangerous!
                    tileDanger[t] += 0.3f;
                }
                else
                {
                    tileDanger[t] += 0.2f;
                }
            }
        }
    }

    virtual protected float GetStonefitness(PlayerStone stone, Tile currentTile, Tile futureTile)
    {
        float fitness = Random.Range(-0.1f, 0.1f);

        if (currentTile == null)
        {
            // We aren't on the board yet
            fitness += 0.20f;
        }





        if (futureTile.PlayerStone != null && futureTile.PlayerStone.PlayerId != stone.PlayerId)
        {
            // There's an enemy 
            fitness += 0.50f;
        }

        if (futureTile.IsScoringSpace == true)
        {
            fitness += 0.50f;
        }

        float currentDanger = 0;
        if (currentTile != null)
        {
            currentDanger = tileDanger[currentTile];
        }

        fitness += currentDanger - tileDanger[futureTile];



        return fitness;
    }


}

