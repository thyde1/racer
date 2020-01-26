using UnityEngine;
using System.Collections;

public class AIInputController : MonoBehaviour
{
    public VehicleController VehicleController;

    private void FixedUpdate()
    {
        this.VehicleController.HandleInput(1, 0);
    }
}
