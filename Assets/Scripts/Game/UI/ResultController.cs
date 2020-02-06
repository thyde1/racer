using UnityEngine;
using UnityEngine.UI;

class ResultController : MonoBehaviour
{
    public VehicleInfo Vehicle { get; set; }

    [SerializeField]
    private Text positionText;
    [SerializeField]
    private Text nameText;

    private void Update()
    {
        this.positionText.text = NumberUtils.GetOrdinal(this.Vehicle.Position);
        this.positionText.color = this.Vehicle.Color;
        this.nameText.text = this.Vehicle.Driver.Name;
        this.nameText.color = this.Vehicle.Color;
    }
}
