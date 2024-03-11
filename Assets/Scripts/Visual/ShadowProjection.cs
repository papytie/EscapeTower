using Dest.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowProjection : MonoBehaviour {

    public GameObject playerObject;
    public GameObject lightObject;
    public float raycastOffsetX = 0f;
    public LayerMask WallLayer => wallLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float positionOffset;
    [SerializeField] float playerWidth;
    [SerializeField] float scaleFactor;


    void Update() {
        Vector2 playerCenter = playerObject.transform.position;
        Vector2 lightPosition = lightObject.transform.position;
        Line2 lightToWallCenter = Line2.CreateFromTwoPoints(lightPosition, playerCenter);

        RaycastHit2D hitwall = Physics2D.Raycast(playerCenter + new Vector2(raycastOffsetX, 0), lightToWallCenter.Direction, float.PositiveInfinity, WallLayer);

        if(!hitwall)
            return;

        // Use thales theorem to find projected shadow width
        Vector2 playerBorder = playerCenter + lightToWallCenter.Direction.Perp() * (playerWidth * 0.5f);
        Line2 wallProjection = Line2.CreatePerpToLineTrhoughPoint(lightToWallCenter, hitwall.point);
        Line2 lightShadowConeBorder = Line2.CreateFromTwoPoints(lightPosition, playerBorder);
        Intersection.FindLine2Line2(ref wallProjection, ref lightShadowConeBorder, out Line2Line2Intr projectionBorder);

        float projectedShadowWidth = playerWidth * Vector2.Distance(lightPosition, projectionBorder.Point) / Vector2.Distance(lightPosition, playerBorder);

        transform.localScale = Vector3.one * (1 + scaleFactor * Mathf.Max(projectedShadowWidth / playerWidth - 1f, 0f));
        transform.position = (Vector3) hitwall.point + Vector3.up * positionOffset;
    }
}
