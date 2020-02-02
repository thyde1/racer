using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    private Camera myCamera;
    private AudioListener audioListener;
    private IEnumerable<VehicleInfo> targets;

    // Start is called before the first frame update
    void Start()
    {
        this.targets = FindObjectsOfType<VehicleInfo>();
        this.myCamera = this.GetComponentInChildren<Camera>();
        this.audioListener = this.myCamera.GetComponentInChildren<AudioListener>();
    }

    public void SetTargets(IEnumerable<VehicleInfo> targets)
    {
        this.targets = targets;
    }

    private void LateUpdate()
    {
        var positions2D = targets.Select(t => Vector3Utils.Make2D(t.transform.position));
        var centrePosition = new Vector3(new[] { positions2D.Min(v => v.x), positions2D.Max(v => v.x) }.Average(), 0, new[] { positions2D.Min(v => v.z), positions2D.Max(v => v.z) }.Average());
        this.myCamera.transform.SetPositionAndRotation(centrePosition + new Vector3(0, 80, -30), Quaternion.Euler(70, 0, 0));
        while (!AreAllTargetsVisible())
        {
            this.myCamera.transform.SetPositionAndRotation(
                this.transform.position - this.transform.forward.normalized * 0.1f,
                Quaternion.Euler(70, 0, 0)
            );
        }

        this.audioListener.transform.position = centrePosition;
    }

    private bool AreAllTargetsVisible()
    {
        var viewpointPositions = targets.Select(t => this.myCamera.WorldToViewportPoint(t.transform.position));
        return !viewpointPositions.Any(p => p.x < 0.1 || p.x > 0.9 || p.y < 0.1 || p.y > 0.9);
    }
}
