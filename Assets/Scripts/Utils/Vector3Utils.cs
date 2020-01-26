using UnityEngine;
using UnityEditor;

public static class Vector3Utils
{
    public static bool IsInvalid(Vector3 vector)
    {
        return (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z));
    }

    public static Vector3 Make2D(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }
}