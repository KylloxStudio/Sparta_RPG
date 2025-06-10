using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 Change(this Vector3 origin, float? x = null, float? y = null, float? z = null)
    {
        float changedX = x == null ? origin.x : x.Value;
        float changedY = y == null ? origin.y : y.Value;
        float changedZ = z == null ? origin.z : z.Value;

        return new Vector3(changedX, changedY, changedZ);
    }
}