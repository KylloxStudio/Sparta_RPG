using UnityEngine;

public static class QuaternionExtensions
{
    public static Quaternion Change(this Quaternion origin, float? x = null, float? y = null, float? z = null)
    {
        float changedX = x == null ? origin.x : x.Value;
        float changedY = y == null ? origin.y : y.Value;
        float changedZ = z == null ? origin.z : z.Value;

        return Quaternion.Euler(changedX, changedY, changedZ);
    }
}