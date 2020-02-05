using UnityEngine;
using UnityEngine.Events;

public class VehicleInfo : MonoBehaviour
{
    public class VehicleEvent : UnityEvent<VehicleInfo> { }

    public int Position { get; set; } = 1;

    public VehicleStatus Status { get; set; } = VehicleStatus.Racing;

    public VehicleEvent OnLapFinished = new VehicleEvent();

    public int CurrentLap { get; private set; }

    public Color Color => VehicleColors.Colors[this.Driver.Player];

    public Driver Driver { get; set; }

    public void IncrementLapCount()
    {
        this.CurrentLap++;
        this.OnLapFinished.Invoke(this);
    }

    public bool IsPlayerControlled => this.Driver.Player > 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
