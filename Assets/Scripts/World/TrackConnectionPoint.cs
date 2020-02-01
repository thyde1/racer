using UnityEngine;

public class TrackConnectionPoint : MonoBehaviour
{
    public enum ConnectionType
    {
        Start,
        End
    }

    public ConnectionType Type { get; set; }
}
