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
        this.vehicle = FindObjectsOfType<VehicleInfo>().Where(v => v.Player == this.PlayerNumber).SingleOrDefault();
    }

    private void Update()
    {
        if (this.vehicle == null)
        {
            lapText.text = string.Empty;
            positionText.text = string.Empty;
            return;
        }

        lapText.text = $"Lap {this.vehicle.CurrentLap}";
        positionText.text = Ordinal.GetOrdinal(this.vehicle.Position);
    }
}
