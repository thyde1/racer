using UnityEngine;

public static class TransformUtils
{
    public static void InvertParentChildRelationship(GameObject parent, GameObject child)
    {
        child.transform.SetParent(null);
        parent.transform.SetParent(child.transform);
    }
}
