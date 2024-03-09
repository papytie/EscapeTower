using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowProjection : MonoBehaviour {

    public GameObject playerObject;
    public GameObject lightObject;
    private float shadowDistance;
    public float raycastOffsetX = 0f;
    public LayerMask WallLayer => wallLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float positionOffset;
    [SerializeField] float playerWidth;
    [SerializeField] float scaleFactor;


    void Update() {
        Vector3 shadowDirection = (playerObject.transform.position - lightObject.transform.position).normalized;
        float lightDistance = Vector3.Distance(playerObject.transform.position, lightObject.transform.position);

        RaycastHit2D hitwall = Physics2D.Raycast(playerObject.transform.position + new Vector3(raycastOffsetX, 0, 0), shadowDirection, float.PositiveInfinity, WallLayer);
        shadowDistance = hitwall.distance;

        transform.position = playerObject.transform.position + shadowDirection * shadowDistance + Vector3.up * positionOffset;

        // Use thales theorem
        Vector3 lightPosition = lightObject.transform.position;
        Vector3 perpendicularDir = Vector2.Perpendicular(shadowDirection);

        Vector3 playerCenter = playerObject.transform.position;
        Vector3 projectedCenter = playerCenter + shadowDirection * shadowDistance;

        Vector3 playerBorder = playerCenter + perpendicularDir * playerWidth * 0.5f;
        LineLineIntersection(out Vector3 projectionBorder, playerBorder, (playerBorder - lightPosition).normalized, projectedCenter, perpendicularDir);

        float projectedSegment = Vector3.Distance(playerCenter, playerBorder) * 2f * Vector3.Distance(lightPosition, projectionBorder) / Vector3.Distance(lightPosition, playerBorder);
        transform.localScale = Vector3.one * (1 + scaleFactor * Mathf.Max(projectedSegment / playerWidth - 1f, 0f));
    }

    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
        Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2) {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if(Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f) {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2)
                    / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        } else {
            intersection = Vector3.zero;
            return false;
        }
    }
}
