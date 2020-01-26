using UnityEngine;
using System.Collections;

public class InputManager
{
    public float GetValue(string axis)
    {
        return Input.GetAxis(axis);
    }
}
