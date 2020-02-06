using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResultsController : MonoBehaviour
{
    private RaceController raceController;
    [SerializeField]
    private VerticalLayoutGroup layout;
    [SerializeField]
    private GameObject result;
    private RaceStatus lastStatus;
    private List<ResultController> resultControllers;

    private void Start()
    {
        this.raceController = FindObjectOfType<RaceController>();
    }

    private void Update()
    {
        if (this.lastStatus != RaceStatus.Finished && this.raceController.Status == RaceStatus.Finished)
        {
            this.resultControllers = new List<ResultController>();
            foreach (var vehicle in this.raceController.Vehicles)
            {
                var result = Instantiate(this.result, this.layout.transform);
                var resultController = result.GetComponent<ResultController>();
                this.resultControllers.Add(resultController);
            }
        }

        if (this.raceController.Status == RaceStatus.Finished)
        {
            var orderedVehicles = this.raceController.Vehicles.OrderBy(v => v.Position);
            for(var i = 0; i < orderedVehicles.Count(); i++)
            {
                this.resultControllers[i].Vehicle = orderedVehicles.ElementAt(i);
            }
        }

        this.lastStatus = this.raceController.Status;
    }
}
