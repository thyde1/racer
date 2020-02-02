using System.Linq;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public float Acceleration = 2;
    public float MaxSpeed = 2;
    public float Drag = 0.5f;
    public float Steering = 4;
    public float Grip = .05f;
    public float NudgeAwayStrength = 0.1f;
    public Vector3 velocity { get; private set; }
    private float acceleratorValue;
    private float steeringValue;
    private AudioSource audioSource;

    public void Start()
    {
        this.audioSource = this.GetComponent<AudioSource>();
    }

    public void HandleInput(float acceleratorValue, float steeringValue)
    {
        this.acceleratorValue = acceleratorValue;
        this.steeringValue = steeringValue;
    }

    public void HandleVehicleCollision(VehicleController other)
    {
        this.velocity += other.velocity;
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

        var velocityAfterAcceleration = degradedVelocity + acceleratorValue * this.Acceleration * Time.deltaTime * this.transform.forward;
        var orthogonalComponent = Vector3.Dot(velocityAfterAcceleration, this.transform.right);
        var velocityAfterTurn = velocityAfterAcceleration - orthogonalComponent * this.transform.right * this.Grip;
        this.velocity = Vector3.ClampMagnitude(velocityAfterTurn, this.MaxSpeed);
        this.transform.Rotate(this.transform.up, this.steeringValue * this.Steering);

        this.HandleCollisions();

        this.transform.Translate(this.velocity, Space.World);

        this.audioSource.volume = this.velocity.magnitude / this.MaxSpeed;
        this.audioSource.pitch = this.velocity.magnitude / this.MaxSpeed;
    }

    private void HandleCollisions()
    {
        var rotationOverlaps = Physics.OverlapBox(this.transform.position, this.transform.lossyScale / 2, this.transform.rotation)
            .Where(this.ShouldCollideWith);
        if (rotationOverlaps.Any())
        {
            foreach (var collider in rotationOverlaps)
            {
                var hitPoint = collider.ClosestPoint(this.transform.position);
                var normalToCollision = (this.transform.position - hitPoint).normalized;
                this.velocity = this.velocity - Vector3.Dot(this.velocity, normalToCollision) * normalToCollision + normalToCollision * this.NudgeAwayStrength;
            }
            return;
        }

        var castHits = Physics.BoxCastAll(this.transform.position, this.transform.lossyScale / 2, this.velocity.normalized, this.transform.rotation, this.velocity.magnitude)
            .Where(hit => this.ShouldCollideWith(hit.collider));
        if (castHits.Any())
        {
            var hit = castHits.OrderBy(h => h.distance).First();
            this.Bounce(hit.collider, hit.normal);
            return;
        }
    }

    private void Bounce(Collider collider, Vector3 normal)
    {
        if (Vector3Utils.IsInvalid(normal))
        {
            return;
        }

        var velocityInNormal = normal * Vector3.Dot(this.velocity, normal.normalized);

        var otherVehicle = collider.GetComponent<VehicleController>();
        if (otherVehicle != null)
        {
            otherVehicle.HandleVehicleCollision(this);
            var updatedVelocity = this.velocity - velocityInNormal;
            this.velocity = new Vector3(updatedVelocity.x, 0, updatedVelocity.z);
        }
        else
        {
            var updatedVelocity = this.velocity - velocityInNormal - Mathf.Max(velocityInNormal.magnitude, this.NudgeAwayStrength) * velocityInNormal.normalized;
            this.velocity = new Vector3(updatedVelocity.x, 0, updatedVelocity.z);
        }
    }

    private bool ShouldCollideWith(Collider collider)
    {
        return collider is BoxCollider && !collider.isTrigger && !collider.gameObject.transform.IsChildOf(this.transform);
    }
}
