using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapKnifeManager : MonoBehaviour
{

   
    //public List<TrapKnifeController> tilesKnives;

   // public List<List<TrapKnifeController>> ListOfLists;
    //public int listCount;


    public List<TrapKnifeController> allKnives;

    public List<TrapKnifeTileController> listOfTrapTiles;

    //public TrapKnifeController kinfe;

    void Start()
    {


        allKnives = new List<TrapKnifeController>();
        listOfTrapTiles = new List<TrapKnifeTileController>();
        
        foreach (Transform child in transform)
        {

            if(child.GetComponent<TrapKnifeTileController>() != null)
            {
                listOfTrapTiles.Add(child.GetComponent<TrapKnifeTileController>());
            }
            
        }

        //TEST
        //kinfe = listOfTrapTiles[1].myKnives[5];

        for(int tileNumber = 0; tileNumber < listOfTrapTiles.Count; tileNumber++)
        {
            for(int knifeNumber = 0; knifeNumber < listOfTrapTiles[tileNumber].myKnives.Count; knifeNumber++)
            {
                allKnives.Add(listOfTrapTiles[tileNumber].myKnives[knifeNumber]);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
