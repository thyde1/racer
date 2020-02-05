using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusHudController : MonoBehaviour
{
    public GameObject LapLabel;
    public GameObject PositionLabel;
    public int PlayerNumber;

    private VehicleInfo vehicle;
    private Text lapText;
    private Text positionText;

    private void Start()
    {
        this.lapText = LapLabel.GetComponent<Text>();
        this.positionText = PositionLabel.GetComponent<Text>();
        this.vehicle = FindObjectsOfType<VehicleInfo>().Where(v => v.Driver.Player == this.PlayerNumber).SingleOrDefault();
        if (this.vehicle == null)
        {
            return;
        }

        this.lapText.color = this.vehicle.Color;
        this.positionText.color = this.vehicle.Color;
    }

    private void Update()
    {
        if (this.vehicle == null)
        {
            this.lapText.text = string.Empty;
            this.positionText.text = string.Empty;
            return;
        }

        this.lapText.text = this.vehicle.Status == VehicleStatus.Finished ? "Finished" : $"Lap {this.vehicle.CurrentLap}";
        this.positionText.text = NumberUtils.GetOrdinal(this.vehicle.Position);
    }
}
