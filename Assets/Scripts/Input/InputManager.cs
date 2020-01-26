using UnityEngine;
using System.Collections;

public class InputManager
{
    public float GetValue(int playerNumber, string axis)
    {
        return Input.GetAxis($"P{playerNumber} {axis}");
    }
}
