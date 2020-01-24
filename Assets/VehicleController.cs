using System.Linq;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public GameObject FrontWheels;
    public GameObject BackWheels;
    public float Acceleration = 1;
    public float MaxSpeed = 1;
    public float Drag = 1;
    public float Steering = 1;
    public float Grip = 1;
    public float NudgeAwayStrength = 1;
    private Camera myCamera;
    private float acceleratorValue;
    private float steeringValue;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        this.myCamera = this.GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var upValue = Input.GetKey(KeyCode.W) ? 1 : 0;
        var downValue = Input.GetKey(KeyCode.S) ? -1 : 0;
        this.acceleratorValue = upValue + downValue;
        var leftValue = Input.GetKey(KeyCode.A) ? -1 : 0;
        var rightValue = Input.GetKey(KeyCode.D) ? 1 : 0;
        this.steeringValue = leftValue + rightValue;
    }

    private void FixedUpdate()
    {
        var velocityDegradation = -this.velocity.normalized * Drag * Time.deltaTime;
        var degradedVelocity = this.velocity + velocityDegradation;
        if (Vector3.Angle(this.velocity, degradedVelocity) > 90)
        {
            // "Drag" only slows, not reverses
            degradedVelocity = Vector3.zero;
        }

        this.velocity = Vector3.ClampMagnitude(degradedVelocity + acceleratorValue * this.Acceleration * Time.deltaTime * this.transform.forward, this.MaxSpeed);
        this.transform.Rotate(this.transform.up, this.steeringValue * this.Steering);
        var rotationOverlaps = Physics.OverlapBox(this.transform.position, this.transform.lossyScale / 2, this.transform.rotation)
            .Where(collider => collider is BoxCollider && !collider.gameObject.transform.IsChildOf(this.transform));
        if (rotationOverlaps.Any())
        {
            var collider = rotationOverlaps.First();
            var hitPoint = collider.ClosestPoint(this.transform.position);
            this.velocity = this.velocity + (this.transform.position - hitPoint).normalized * this.NudgeAwayStrength;
            this.transform.Translate(this.velocity, Space.World);
            return;
        }

        var castHits = Physics.BoxCastAll(this.transform.position, this.transform.lossyScale / 2, this.velocity.normalized, this.transform.rotation, this.velocity.magnitude)
            .Where(hit => !hit.collider.gameObject.transform.IsChildOf(this.transform));
        if (castHits.Any())
        {
            this.Bounce(castHits.First().normal);
            return;
        }

        this.transform.Translate(this.velocity, Space.World);
    }

    private void LateUpdate()
    {
        this.myCamera.transform.SetPositionAndRotation(this.myCamera.transform.position, Quaternion.Euler(90, 0, 0));
    }

    private void Bounce(Vector3 normal)
    {
        var updatedVelocity = this.velocity - 2 * normal * Vector3.Dot(this.velocity, normal);
        this.velocity = new Vector3(updatedVelocity.x, 0, updatedVelocity.z);
    }
}
