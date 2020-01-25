using UnityEngine;
using UnityEditor;

public static class Vector3Extensions
{
    public static bool IsInvalid(this Vector3 vector)
    {
        return (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z));
    }
}