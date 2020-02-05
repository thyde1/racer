﻿using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class RaceController : MonoBehaviour
{
    public GameObject vehicle;
    public int Players { private get; set; }
    public int Laps { private get; set; }
    public TimeSpan time { get; private set; }
    public RaceStatus Status { get; private set; } = RaceStatus.PreRace;

    public VehicleInfo[] Vehicles { get; private set; }
    private GameObject finishedMenu;

    // Use this for initialization
    void Start()
    {
        this.InstantiateVehicles();
        GetComponent<CheckpointWatcher>().enabled = true;
        this.time = TimeSpan.FromSeconds(-3);
        this.AssignDriversToVehicles();
        var hudController = FindObjectOfType<HudController>();
        this.finishedMenu = FindObjectOfType<FinishedMenuController>().gameObject;
        this.finishedMenu.SetActive(false);
        var playerVehicles = this.Vehicles.Where(v => v.IsPlayerControlled);
        var camera = FindObjectOfType<CameraController>();
        camera.SetTargets(playerVehicles.Any() ? playerVehicles : this.Vehicles);
        hudController.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        this.time = this.time.Add(TimeSpan.FromSeconds(Time.deltaTime));
        if (this.Status == RaceStatus.PreRace && this.time > TimeSpan.Zero)
        {
            this.ActivateVehicles();
            this.Status = RaceStatus.InProgress;
        }
    }

    private void AssignDriversToVehicles()
    {
        for (var vehicleNumber = 0; vehicleNumber < this.Vehicles.Count(); vehicleNumber++)
        {
            var vehicle = Vehicles[vehicleNumber];
            if (vehicleNumber < this.Players)
            {
                
                vehicle.Driver = new Driver(vehicleNumber + 1);
            }
            else
            {
                vehicle.Driver = new Driver(0);
            }

            vehicle.GetComponentInChildren<MeshRenderer>().material.color = vehicle.Color;
        }
    }

    private void ActivateVehicles()
    {
        var playerVehicles = Vehicles.Where(v => v.GetComponent<VehicleInfo>().IsPlayerControlled);
        foreach (var playerVehicle in playerVehicles)
        {
            var playerInputController = playerVehicle.gameObject.AddComponent<PlayerInputController>();
            playerInputController.VehicleController = playerVehicle.GetComponent<VehicleController>();
            playerInputController.PlayerNumber = playerVehicle.GetComponent<VehicleInfo>().Driver.Player;
        }

        foreach (var vehicle in Vehicles.Except(playerVehicles))
        {
            vehicle.gameObject.AddComponent<AIInputController>().VehicleController = vehicle.GetComponent<VehicleController>();
        }
    }

    private void CheckForRaceFinished(VehicleInfo vehicle)
    {
        if (vehicle.CurrentLap > this.Laps)
        {
            vehicle.Status = VehicleStatus.Finished;
        }
        var playerVehicles = this.Vehicles.Where(v => v.IsPlayerControlled);
        if ((playerVehicles.Any() ? playerVehicles : this.Vehicles).All(v => v.Status == VehicleStatus.Finished))
        {
            this.Status = RaceStatus.Finished;
            this.finishedMenu.SetActive(true);
        }
    }

    private void InstantiateVehicles()
    {
        var checkpointWatcher = GetComponent<CheckpointWatcher>();
        var firstCheckpoint = checkpointWatcher.Checkpoints.First();
        for (var i = 0; i < 12; i ++)
        {
            var newVehicle = Instantiate(
                vehicle,
                firstCheckpoint.transform.position - firstCheckpoint.transform.forward * 5 * i + firstCheckpoint.transform.right * (i % 2 == 0 ? -5 : 5),
                firstCheckpoint.transform.rotation
            );
            SceneManager.MoveGameObjectToScene(newVehicle, this.gameObject.scene);
        }

        this.Vehicles = FindObjectsOfType<VehicleInfo>().OrderBy(v => v.Position).ToArray();
        foreach (var vehicle in this.Vehicles)
        {
            vehicle.OnLapFinished.AddListener(this.CheckForRaceFinished);
        }
    }
}
