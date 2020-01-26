using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour
{
    public VehicleController VehicleController;
    private InputManager inputManager;

    private void Start()
    {
        this.inputManager = new InputManager();
    }

    // Update is called once per frame
    void Update()
    {
        var acceleratorValue = inputManager.GetValue(Buttons.Accelerator);
        var steeringValue = inputManager.GetValue(Buttons.Steering);

        this.VehicleController.HandleInput(acceleratorValue, steeringValue);
    }
}
