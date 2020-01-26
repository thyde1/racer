using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour
{
    public VehicleController VehicleController;
    
    // Update is called once per frame
    void Update()
    {
        var upValue = Input.GetKey(KeyCode.W) ? 1 : 0;
        var downValue = Input.GetKey(KeyCode.S) ? -1 : 0;
        var acceleratorValue = upValue + downValue;
        var leftValue = Input.GetKey(KeyCode.A) ? -1 : 0;
        var rightValue = Input.GetKey(KeyCode.D) ? 1 : 0;
        var steeringValue = leftValue + rightValue;

        this.VehicleController.HandleInput(acceleratorValue, steeringValue);
    }
}
