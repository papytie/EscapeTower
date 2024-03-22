using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrapKnifeController : MonoBehaviour
{

    public float delay;
    public bool isNearKnifeActivated;

    private Animation myAnim;

    private TrapKnifeManager trapKnifeManager;
    private PlayerController player;
    public List<TrapKnifeController> listOfNearKnives;
    private bool init;
    public float nearDistance;
    public bool isPlayerNear;

    void Start()
    {
        listOfNearKnives = new List<TrapKnifeController>();
        trapKnifeManager = FindObjectOfType<TrapKnifeManager>();
        player = FindObjectOfType<PlayerController>();
        myAnim = gameObject.GetComponent<Animation>();
        isNearKnifeActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            FindNearKnives();
            init = true;
        }

        if (Vector2.Distance(player.transform.position, transform.position) <= nearDistance)
        {
            isPlayerNear = true;
        }
        else
        {
            isPlayerNear = false;
        }


        if(isPlayerNear || isNearKnifeActivated)
        {
            StartCoroutine(KnifeActivation());
        }
    }


    public void FindNearKnives()
    {
       foreach (TrapKnifeController knife in trapKnifeManager.allKnives)
        {
            if (Vector2.Distance(knife.transform.position, transform.position) <= nearDistance)
            {
                listOfNearKnives.Add(knife);
            }
        }
        listOfNearKnives.Remove(this);
    }

    IEnumerator KnifeActivation()
    {
        if(myAnim.isPlaying  == false)
        {
            myAnim.Play("TrapKnifeAnim");
            foreach (TrapKnifeController knife in listOfNearKnives)
            {
                yield return new WaitForSeconds(delay);
                knife.isNearKnifeActivated = true;
            }
        }
        isNearKnifeActivated = false;
    }
}
