using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowProjection : MonoBehaviour
{
    public GameObject playerObect;
    public GameObject lightObject;
    private float shadowDistance;
    public float raycastOffsetX = 0f;
    public LayerMask WallLayer => wallLayer;
    [SerializeField] LayerMask wallLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
        
    
        Vector3 shadowDirection = (playerObect.transform.position - lightObject.transform.position).normalized;

        RaycastHit2D hitwall = Physics2D.Raycast(playerObect.transform.position + new Vector3(raycastOffsetX,0,0), shadowDirection, float.PositiveInfinity, WallLayer);
        shadowDistance = hitwall.distance;

        transform.position = playerObect.transform.position + shadowDirection * shadowDistance;

  
    }
}
