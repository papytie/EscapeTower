using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector2 ToVector2(this Vector3 v)
    {
        return new(v.x, v.y);
    }


    /// <summary>
    /// Rotate angle degrees the point around the pivot (Z-axis) 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="pivot"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 RotatePointAroundPivot2D(this Vector3 point, Vector3 pivot, float angle) {
        angle = Mathf.Deg2Rad * angle;

        float X = point.x - pivot.x;
        float Y = point.y - pivot.y;

        float cosAngle = Mathf.Cos(angle);
        float sinAngle = Mathf.Sin(angle);

        point.x = pivot.x + X * cosAngle - Y * sinAngle;
        point.y = pivot.y + X * sinAngle + Y * cosAngle;

        return point;
    }
}
