using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public GameObject TimeLabel;
    public GameObject LapLabel;
    public GameObject PositionLabel;
    public GameObject CountdownLabel;
    public VehicleInfo Vehicle { private get; set; }

    private RaceController raceController;
    private Text timeText;
    private Text lapText;
    private Text positionText;
    private Text countdownText;

    // Start is called before the first frame update
    void Start()
    {
        this.raceController = FindObjectOfType<RaceController>();
        this.timeText = TimeLabel.GetComponent<Text>();
        this.lapText = LapLabel.GetComponent<Text>();
        this.positionText = PositionLabel.GetComponent<Text>();
        this.countdownText = CountdownLabel.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        var countdownNumber = Math.Ceiling(raceController.time.Negate().TotalMilliseconds / 1000);
        this.countdownText.text = countdownNumber == 0 ? "GO!" : (countdownNumber > 0 ? countdownNumber.ToString("0") : "");

        var time = raceController.time < TimeSpan.Zero ? TimeSpan.Zero : raceController.time;
        timeText.text = $"{time.ToString(@"mm\:ss\.ff")}";
        lapText.text = $"Lap {this.Vehicle.CurrentLap}";
        positionText.text = Ordinal.GetOrdinal(this.Vehicle.Position);
    }
}
