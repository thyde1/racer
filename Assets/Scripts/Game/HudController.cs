using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public GameObject TimeLabel;
    public GameObject CountdownLabel;
    public StatusHudController[] StatusHudControllers;

    private RaceController raceController;
    private IEnumerable<VehicleInfo> playerVehicles;
    private Text timeText;
    private Text countdownText;

    // Start is called before the first frame update
    void Start()
    {
        this.raceController = FindObjectOfType<RaceController>();
        this.playerVehicles = FindObjectsOfType<VehicleInfo>().Where(v => v.IsPlayerControlled);
        this.timeText = TimeLabel.GetComponent<Text>();
        this.countdownText = CountdownLabel.GetComponent<Text>();
        foreach (var controller in this.GetComponentsInChildren<StatusHudController>())
        {
            controller.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var countdownNumber = Math.Ceiling(raceController.time.Negate().TotalMilliseconds / 1000);
        this.countdownText.text = countdownNumber == 0 ? "GO!" : (countdownNumber > 0 ? countdownNumber.ToString("0") : "");

        var time = raceController.time < TimeSpan.Zero ? TimeSpan.Zero : raceController.time;
        timeText.text = $"{time.ToString(@"mm\:ss\.ff")}";
        if (this.raceController.Status == RaceStatus.Finished && this.playerVehicles.Count() == 1)
        {
            this.countdownText.text = $"You finished {Ordinal.GetOrdinal(this.playerVehicles.Single().Position)}";
        }
    }
}
