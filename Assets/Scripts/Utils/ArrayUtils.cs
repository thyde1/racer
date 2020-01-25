using UnityEngine;
using UnityEditor;
using System;

public static class ArrayUtils
{
    public static T GetNextWrapped<T>(T[] array, T current)
    {
        var currentIndex = Array.IndexOf(array, current);
        if (currentIndex == array.Length - 1)
        {
            return array[0];
        }

        return array[currentIndex + 1];
    }
}
