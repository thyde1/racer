using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Checkpoint : MonoBehaviour
{
    public IEnumerable<VehicleInfo> Vehicles;
    public CheckpointWatcher CheckpointWatcher;

    private void OnTriggerEnter(Collider other)
    {
        if (this.Vehicles.Select(v => v.gameObject).Contains(other.gameObject))
        {
            var vehicleInfo = other.gameObject.GetComponent<VehicleInfo>();
            this.CheckpointWatcher.VehiclePassedCheckpoint(this.gameObject, vehicleInfo);
        }
    }
}
