using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class RaceController : MonoBehaviour
{
    private IEnumerable<VehicleController> vehicles;
    private CheckpointWatcher checkpointWatcher;

    public TimeSpan time { get; private set; }

    public int CurrentLap => this.checkpointWatcher.currentLaps.Values.First();

    // Use this for initialization
    void Start()
    {
        this.vehicles = FindObjectsOfType<VehicleController>();
        this.checkpointWatcher = FindObjectOfType<CheckpointWatcher>();
        this.time = TimeSpan.FromSeconds(-3);
    }

    // Update is called once per frame
    void Update()
    {
        this.time = this.time.Add(TimeSpan.FromSeconds(Time.deltaTime));
        if (this.time > TimeSpan.Zero)
        {
            this.ActivateVehicles();
        }
    }

    private void ActivateVehicles()
    {
        foreach (var vehicle in this.vehicles)
        {
            vehicle.enabled = true;
        }
    }
}
