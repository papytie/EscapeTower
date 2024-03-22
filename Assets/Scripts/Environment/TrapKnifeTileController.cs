using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapKnifeTileController : MonoBehaviour
{

    public List<TrapKnifeController> myKnives;


    private void Awake()
    {

        myKnives = new List<TrapKnifeController>();

        Debug.Log(transform.childCount);
        foreach (Transform child in transform)
        {
            myKnives.Add(child.GetComponent<TrapKnifeController>());
            
        }
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
