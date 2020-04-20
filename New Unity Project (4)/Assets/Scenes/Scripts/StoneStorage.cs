using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneStorage : MonoBehaviour
{

    // Start is called before the first frame update
    public Tile StartingTile;
    void Start()
    {
        //create one stone for each placeholder spot
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject theStone = Instantiate(StonePrefab);
            theStone.GetComponent<PlayerStone>().StartingTile = this.StartingTile;
            theStone.GetComponent<PlayerStone>().MyStoneStorage = this;

            //instantiate a new copy of the stone prefab
            AddStoneToStorage(theStone, this.transform.GetChild(i));
        }
    }
    public GameObject StonePrefab;

    // Update is called once per frame

    public void AddStoneToStorage(GameObject theStone, Transform thePlaceholder = null)
    {
        if (thePlaceholder == null)
        {
            //find the first empty placeholder.
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform p = this.transform.GetChild(i);
                if (p.childCount == 0)
                {
                    //this placeholder is empty!
                    thePlaceholder = p;
                    break;//break out of the loop.
                }
            }
            if (thePlaceholder == null)
            {
                
                return;

            }
        }
        //parent the stone to the placeholder
        theStone.transform.SetParent(thePlaceholder);

        //reset the stone's local position to 0,0,0
        theStone.transform.localPosition = Vector3.zero;
    }



}
