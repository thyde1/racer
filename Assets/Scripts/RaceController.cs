using UnityEngine;
using System;
using System.Linq;

public class RaceController : MonoBehaviour
{
    public int Players { private get; set; }
    public int Laps { private get; set; }

    public TimeSpan time { get; private set; }

    private VehicleInfo[] vehicles;
    private bool raceStarted = false;

    // Use this for initialization
    void Start()
    {
        this.vehicles = FindObjectsOfType<VehicleInfo>().OrderBy(v => v.Position).ToArray();
        this.time = TimeSpan.FromSeconds(-3);
        this.AssignPlayersToVehicles();
        var player1Vehicle = this.vehicles.SingleOrDefault(v => v.Player == 1);
        var hudController = FindObjectOfType<HudController>();
        var playerVehicles = this.vehicles.Where(v => v.Player > 0);
        var camera = FindObjectOfType<CameraController>();
        camera.SetTargets(playerVehicles.Any() ? playerVehicles : vehicles);
        hudController.Vehicle = player1Vehicle ?? this.vehicles.First();
        hudController.enabled = true;
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

    private void AssignPlayersToVehicles()
    {
        for (var vehicleNumber = 0; vehicleNumber < this.vehicles.Count(); vehicleNumber++)
        {
            var vehicle = vehicles[vehicleNumber];
            if (vehicleNumber < this.Players)
            {
                
                vehicle.Player = vehicleNumber + 1;
                vehicle.GetComponentInChildren<MeshRenderer>().material.color = VehicleColors.PlayerColors[vehicleNumber];
            }
            else
            {
                vehicle.Player = 0;
                vehicle.GetComponentInChildren<MeshRenderer>().material.color = VehicleColors.AIColor;
            }
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
