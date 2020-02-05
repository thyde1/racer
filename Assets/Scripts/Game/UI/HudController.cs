using System;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public GameObject TimeLabel;
    public GameObject CountdownLabel;
    public StatusHudController[] StatusHudControllers;

    private RaceController raceController;
    private Text timeText;
    private Text countdownText;
    private AudioSource beeper;
    private int countdownNumber;

    // Start is called before the first frame update
    void Start()
    {
        this.raceController = FindObjectOfType<RaceController>();
        this.timeText = this.TimeLabel.GetComponent<Text>();
        this.countdownText = this.CountdownLabel.GetComponent<Text>();
        this.beeper = this.GetComponentInChildren<AudioSource>();
        foreach (var controller in this.GetComponentsInChildren<StatusHudController>())
        {
            controller.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var newCountdownNumber = Mathf.CeilToInt(Convert.ToSingle(raceController.time.Negate().TotalMilliseconds / 1000));
        if (newCountdownNumber != this.countdownNumber && newCountdownNumber >= 0)
        {
            this.beeper.pitch = newCountdownNumber == 0 ? 1.5f : 1;
            this.beeper.Play();
        }

        this.countdownNumber = newCountdownNumber;
        this.countdownText.text = countdownNumber == 0 ? "GO!" : (countdownNumber > 0 ? countdownNumber.ToString("0") : "");

        var time = raceController.time < TimeSpan.Zero ? TimeSpan.Zero : raceController.time;
        timeText.text = $"{time.ToString(@"mm\:ss\.ff")}";
    }
}
