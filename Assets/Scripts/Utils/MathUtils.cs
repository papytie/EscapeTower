using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils {

    #region Clamp
    public static float ClampAngle(float angle) {
        return ClampLoopableValue(angle, -180f, 180f);
    }

    public static float ClampProgress(float progress) {
        return ClampLoopableValue(progress, 0f, 1f);
    }

    public static float ClampLoopableValue(float value, float minValue, float maxValue) {
        float loopValue = maxValue - minValue;
        while(value < minValue)
            value += loopValue;
        while(value > maxValue)
            value -= loopValue;
        return value;
    }
    #endregion

    #region Trigonometry
    public static float AngleFromCircle(Vector3 direction) {
        return AngleFromCircle(Vector3.zero, direction);
    }

    public static float AngleFromCircle(Vector3 center, Vector3 position) {
        float angle = SignedAngleFromCircle(center, position);
        return angle < 0 ? angle + 360 : angle;
    }

    public static float SignedAngleFromCircle(Vector3 direction) {
        return SignedAngleFromCircle(Vector3.zero, direction);
    }

    public static float SignedAngleFromCircle(Vector3 center, Vector3 position) {
        return Mathf.Atan2(position.y - center.y, position.x - center.x) * Mathf.Rad2Deg;
    }

    public static Vector3 DirectionFromAngle(float angle) {
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
    }
    #endregion
}