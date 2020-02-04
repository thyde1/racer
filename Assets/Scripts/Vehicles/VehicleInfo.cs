using UnityEngine;
using UnityEngine.Events;

public class VehicleInfo : MonoBehaviour
{
    public class VehicleEvent : UnityEvent<VehicleInfo> { }

    public int Player
    {
        get
        {
            return this.player;
        }
        set
        {
            if (value == 0)
            {
                this.DriverName = "AI";
            }
            else
            {
                this.DriverName = $"Player {NumberUtils.NumberToWords(value)}";
            }
            this.player = value;
        }
    }

    private int player = 0;

    public int Position { get; set; } = 1;

    public VehicleStatus Status { get; set; } = VehicleStatus.Racing;

    public VehicleEvent OnLapFinished = new VehicleEvent();

    public int CurrentLap { get; private set; }

    public Color Color => VehicleColors.Colors[this.Player];

    public string DriverName { get; private set; }

    public void IncrementLapCount()
    {
        this.CurrentLap++;
        this.OnLapFinished.Invoke(this);
    }

    public bool IsPlayerControlled => this.Player > 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
