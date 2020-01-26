using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIInputController : MonoBehaviour
{
    public VehicleController VehicleController;

    private VehicleInfo vehicleInfo;
    private Dictionary<VehicleInfo, GameObject> nextCheckpoints;

    private void Start()
    {
        this.vehicleInfo = this.GetComponent<VehicleInfo>();
        this.nextCheckpoints = FindObjectOfType<CheckpointWatcher>().nextCheckpoints;
    }

    private void FixedUpdate()
    {
        var nextCheckpoint = this.nextCheckpoints[this.vehicleInfo];
        var angleToNextCheckpoint = Vector3.SignedAngle(Vector3.forward, nextCheckpoint.transform.position - this.transform.position, Vector3.up);
        var angleToTurnToNextCheckpoint = AngleUtils.GetSignedSmallestAngleBetween(this.transform.rotation.eulerAngles.y, angleToNextCheckpoint);
        this.VehicleController.HandleInput(1, Mathf.Clamp(angleToTurnToNextCheckpoint, -1, 1));
    }
}
