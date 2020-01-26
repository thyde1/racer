using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour
{
    public VehicleController VehicleController;
    public int PlayerNumber;
    private InputManager inputManager;

    private void Start()
    {
        this.inputManager = new InputManager();
    }

    // Update is called once per frame
    void Update()
    {
        var acceleratorValue = inputManager.GetValue(this.PlayerNumber, Buttons.Accelerator);
        var steeringValue = inputManager.GetValue(this.PlayerNumber, Buttons.Steering);

        this.VehicleController.HandleInput(acceleratorValue, steeringValue);
    }
}
