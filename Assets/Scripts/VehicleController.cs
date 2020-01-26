﻿using System.Linq;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
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

    public void HandleInput(float acceleratorValue, float steeringValue)
    {
        this.acceleratorValue = acceleratorValue;
        this.steeringValue = steeringValue;
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
        var rotationOverlaps = Physics.OverlapBox(this.transform.position, this.transform.lossyScale / 2, this.transform.rotation)
            .Where(this.ShouldCollideWith);
        if (rotationOverlaps.Any())
        {
            var collider = rotationOverlaps.First();
            var hitPoint = collider.ClosestPoint(this.transform.position);
            var normalToCollision = (this.transform.position - hitPoint).normalized;
            this.velocity = this.velocity - Vector3.Dot(this.velocity, normalToCollision) * normalToCollision + normalToCollision * this.NudgeAwayStrength;
            this.transform.Translate(this.velocity, Space.World);
            return;
        }

        var castHits = Physics.BoxCastAll(this.transform.position, this.transform.lossyScale / 2, this.velocity.normalized, this.transform.rotation, this.velocity.magnitude)
            .Where(hit => this.ShouldCollideWith(hit.collider));
        if (castHits.Any())
        {
            this.Bounce(castHits.First().normal);
            return;
        }

        this.transform.Translate(this.velocity, Space.World);
    }

    private void LateUpdate()
    {
        if (this.myCamera == null)
        {
            return;
        }

        this.myCamera.transform.SetPositionAndRotation(this.myCamera.transform.position, Quaternion.Euler(90, 0, 0));
    }

    private void Bounce(Vector3 normal)
    {
        if (Vector3Utils.IsInvalid(normal))
        {
            return;
        }

        var velocityInNormal = normal * Vector3.Dot(this.velocity, normal.normalized);
        var updatedVelocity = this.velocity - velocityInNormal - Mathf.Max(velocityInNormal.magnitude, this.NudgeAwayStrength) * velocityInNormal.normalized;
        this.velocity = new Vector3(updatedVelocity.x, 0, updatedVelocity.z);
        this.transform.Translate(this.velocity, Space.World);
    }

    private bool ShouldCollideWith(Collider collider)
    {
        return collider is BoxCollider && !collider.isTrigger && !collider.gameObject.transform.IsChildOf(this.transform);
    }
}