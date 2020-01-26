using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    private Camera myCamera;
    private IEnumerable<VehicleInfo> targets;

    // Start is called before the first frame update
    void Start()
    {
        this.myCamera = this.GetComponentInChildren<Camera>();
        this.targets = FindObjectsOfType<VehicleInfo>().Where(v => v.Player > 0);
    }

    private void LateUpdate()
    {
        var positions2D = targets.Select(t => Vector3Utils.Make2D(t.transform.position));
        var centrePosition = new Vector3(new[] { positions2D.Min(v => v.x), positions2D.Max(v => v.x) }.Average(), 0, new[] { positions2D.Min(v => v.z), positions2D.Max(v => v.z) }.Average());
        this.myCamera.transform.SetPositionAndRotation(centrePosition + new Vector3(0, 80, -30), Quaternion.Euler(70, 0, 0));
    }
}
