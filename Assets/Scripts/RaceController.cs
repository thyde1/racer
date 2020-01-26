using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class RaceController : MonoBehaviour
{
    private IEnumerable<VehicleController> vehicles;
    private bool raceStarted = false;

    public TimeSpan time { get; private set; }

    // Use this for initialization
    void Start()
    {
        this.vehicles = FindObjectsOfType<VehicleController>();
        this.time = TimeSpan.FromSeconds(-3);
        FindObjectOfType<HudController>().Vehicle = this.vehicles.SingleOrDefault(v => v.GetComponent<VehicleInfo>().Player == 1).GetComponent<VehicleInfo>();
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
        var player1Vehicle = vehicles.SingleOrDefault(v => v.GetComponent<VehicleInfo>().Player == 1);
        if (player1Vehicle == null)
        {
            throw new Exception("There was no player one vehicle");
        }

        player1Vehicle.gameObject.AddComponent<PlayerInputController>().VehicleController = player1Vehicle;
        foreach (var vehicle in vehicles.Except(new[] { player1Vehicle }))
        {
            vehicle.gameObject.AddComponent<AIInputController>().VehicleController = vehicle;
        }
    }
}
