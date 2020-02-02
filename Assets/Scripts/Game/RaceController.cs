using UnityEngine;
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

    private VehicleInfo[] vehicles;
    private GameObject finishedMenu;

    // Use this for initialization
    void Start()
    {
        this.InstantiateVehicles();
        GetComponent<CheckpointWatcher>().enabled = true;
        this.time = TimeSpan.FromSeconds(-3);
        this.AssignPlayersToVehicles();
        var player1Vehicle = this.vehicles.SingleOrDefault(v => v.Player == 1);
        var hudController = FindObjectOfType<HudController>();
        this.finishedMenu = FindObjectOfType<FinishedMenuController>().gameObject;
        this.finishedMenu.SetActive(false);
        var playerVehicles = this.vehicles.Where(v => v.IsPlayerControlled);
        var camera = FindObjectOfType<CameraController>();
        camera.SetTargets(playerVehicles.Any() ? playerVehicles : vehicles);
        hudController.Vehicle = player1Vehicle ?? this.vehicles.First();
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
        var playerVehicles = vehicles.Where(v => v.GetComponent<VehicleInfo>().IsPlayerControlled);
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

    private void CheckForRaceFinished(VehicleInfo vehicle)
    {
        if (vehicle.CurrentLap > this.Laps)
        {
            vehicle.Status = VehicleStatus.Finished;
        }
        if (this.vehicles.Where(v => v.IsPlayerControlled).All(v => v.Status == VehicleStatus.Finished))
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
                firstCheckpoint.transform.position - firstCheckpoint.transform.forward * 7 * i + firstCheckpoint.transform.right * (i % 2 == 0 ? -5 : 5),
                firstCheckpoint.transform.rotation
            );
            SceneManager.MoveGameObjectToScene(newVehicle, this.gameObject.scene);
        }

        this.vehicles = FindObjectsOfType<VehicleInfo>().OrderBy(v => v.Position).ToArray();
        foreach (var vehicle in this.vehicles)
        {
            vehicle.OnLapFinished.AddListener(this.CheckForRaceFinished);
        }
    }
}
