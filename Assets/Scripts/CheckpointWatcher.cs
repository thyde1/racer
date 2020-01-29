﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class CheckpointWatcher : MonoBehaviour
{
    public GameObject[] Checkpoints;
    public Dictionary<VehicleInfo, GameObject> nextCheckpoints { get; private set; }
    private IEnumerable<VehicleInfo> vehicles;

    // Use this for initialization
    void Start()
    {
        this.HideCheckpoints();
        this.vehicles = FindObjectsOfType<VehicleInfo>();
        this.nextCheckpoints = this.vehicles.ToDictionary(v => v.GetComponent<VehicleInfo>(), v => this.Checkpoints.First());
        foreach (var c in this.Checkpoints)
        {
            var checkpoint = c.AddComponent<Checkpoint>();
            checkpoint.Vehicles = this.vehicles;
            checkpoint.CheckpointWatcher = this;
        }

        this.CalculatePositions();
    }

    private void FixedUpdate()
    {
        this.CalculatePositions();
    }

    private void CalculatePositions()
    {
        var orderedVehicles = this.vehicles
            .OrderByDescending(v => v.CurrentLap)
            .ThenByDescending(v => {
                var checkpointIndex = Array.IndexOf(this.Checkpoints, nextCheckpoints[v]);
                return checkpointIndex == 0 ? this.Checkpoints.Count() : checkpointIndex; // If next checkpoint is 0, better than next checkpoint is 5
            })
            .ThenBy(v => Vector3.Distance(v.transform.position, nextCheckpoints[v].transform.position));
        var position = 0;
        foreach (var vehicle in orderedVehicles)
        {
            position++;
            vehicle.Position = position;
        }
    }

    public void VehiclePassedCheckpoint(GameObject checkpoint, VehicleInfo vehicleInfo)
    {
        if (nextCheckpoints[vehicleInfo] == checkpoint)
        {
            nextCheckpoints[vehicleInfo] = ArrayUtils.GetNextWrapped(this.Checkpoints, checkpoint);
            if (nextCheckpoints[vehicleInfo] == this.Checkpoints[1])
            {
                vehicleInfo.CurrentLap++;
            }
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
