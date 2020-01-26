using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class CheckpointWatcher : MonoBehaviour
{
    public GameObject[] Checkpoints;
    public Dictionary<VehicleInfo, GameObject> nextCheckpoints { get; private set; }
    public Dictionary<VehicleInfo, int> currentLaps { get; private set; }
    private IEnumerable<GameObject> vehicles;

    // Use this for initialization
    void Start()
    {
        this.HideCheckpoints();
        this.vehicles = FindObjectsOfType<VehicleInfo>().Select(v => v.gameObject);
        this.nextCheckpoints = this.vehicles.ToDictionary(v => v.GetComponent<VehicleInfo>(), v => this.Checkpoints.First());
        this.currentLaps = this.vehicles.ToDictionary(v => v.GetComponent<VehicleInfo>(), v => 0);
        foreach (var c in this.Checkpoints)
        {
            var checkpoint = c.AddComponent<Checkpoint>();
            checkpoint.Vehicles = this.vehicles;
            checkpoint.CheckpointWatcher = this;
        }
    }

    internal void VehiclePassedCheckpoint(GameObject checkpoint, VehicleInfo vehicleInfo)
    {
        var checkpointIndex = Array.IndexOf(this.Checkpoints, checkpoint);
        if (nextCheckpoints[vehicleInfo] == checkpoint)
        {
            nextCheckpoints[vehicleInfo] = ArrayUtils.GetNextWrapped(this.Checkpoints, checkpoint);
            if (nextCheckpoints[vehicleInfo] == this.Checkpoints[1])
            {
                currentLaps[vehicleInfo]++;
            }
            Debug.Log($"Checkpoint {checkpointIndex + 1}");
        }
    }

    private void HideCheckpoints()
    {
        foreach(var checkpoint in this.Checkpoints)
        {
            var renderer = checkpoint.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            renderer.enabled = false;
        }
    }
}
