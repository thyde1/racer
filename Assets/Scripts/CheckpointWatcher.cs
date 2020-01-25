using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class CheckpointWatcher : MonoBehaviour
{
    public GameObject[] Checkpoints;
    private object nextCheckpoint;
    private IEnumerable<GameObject> vehicles;

    // Use this for initialization
    void Start()
    {
        this.HideCheckpoints();
        this.nextCheckpoint = this.Checkpoints.First();
        this.vehicles = FindObjectsOfType<VehicleInfo>().Select(v => v.gameObject);
        foreach (var c in this.Checkpoints)
        {
            var checkpoint = c.AddComponent<Checkpoint>();
            checkpoint.Vehicles = this.vehicles;
            checkpoint.CheckpointWatcher = this;
        }
    }

    internal void VehiclePassedCheckpoint(GameObject checkpoint, VehicleInfo vehicleInfo)
    {
        Debug.Log($"Checkpoint {Array.IndexOf(this.Checkpoints, checkpoint) + 1}");
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
