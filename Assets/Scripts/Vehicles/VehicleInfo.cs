using UnityEngine;
using UnityEngine.Events;

public class VehicleInfo : MonoBehaviour
{
    public int Player = 1;
    public int Position = 1;

    public UnityEvent OnLapFinished;

    private int currentLap = 0;
    public int CurrentLap
    {
        get
        {
            return this.currentLap;
        }
        set
        {
            this.currentLap = value;
            this.OnLapFinished.Invoke();
        }
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
