using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResultsController : MonoBehaviour
{
    private RaceController raceController;
    private Text text;

    private void Start()
    {
        this.raceController = FindObjectOfType<RaceController>();
        this.text = this.GetComponent<Text>();
    }

    private void Update()
    {
        if (this.raceController.Status == RaceStatus.Finished)
        {
            this.text.enabled = true;
            this.text.text = string.Join("\n", this.raceController.Vehicles.OrderBy(v => v.Position).Select(v => $"{NumberUtils.GetOrdinal(v.Position)}\t{v.Driver.Name}"));
        }
        else
        {
            this.text.enabled = false;
        }
    }
}
