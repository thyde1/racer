using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class RaceController : MonoBehaviour
{
    public int Players { private get; set; }
    public int Laps { private get; set; }

    public TimeSpan time { get; private set; }

    private IEnumerable<VehicleInfo> vehicles;
    private bool raceStarted = false;

    // Use this for initialization
    void Start()
    {
        this.vehicles = FindObjectsOfType<VehicleInfo>();
        this.time = TimeSpan.FromSeconds(-3);
        FindObjectOfType<HudController>().Vehicle = this.vehicles.SingleOrDefault(v => v.Player == 1);
    }

    // Update is called once per frame
    void Update()
    {
        this.time = this.time.Add(TimeSpan.FromSeconds(Time.deltaTime));
        if (!raceStarted && this.time > TimeSpan.Zero)
        {
            this.ActivateVehicles();
            this.raceStarted = true;
        }
    }

    private void ActivateVehicles()
    {
        var playerVehicles = vehicles.Where(v => v.GetComponent<VehicleInfo>().Player > 0);
        foreach (var playerVehicle in playerVehicles)
        {
            var playerInputController = playerVehicle.gameObject.AddComponent<PlayerInputController>();
            playerInputController.VehicleController = playerVehicle.GetComponent<VehicleController>();
            playerInputController.PlayerNumber = playerVehicle.GetComponent<VehicleInfo>().Player;
        }

        foreach (var vehicle in vehicles.Except(playerVehicles))
        {
            vehicle.gameObject.AddComponent<AIInputController>().VehicleController = vehicle.GetComponent<VehicleController>();
        }
    }
}
