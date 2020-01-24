using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public GameObject FrontWheels;
    public GameObject BackWheels;
    public float Acceleration = 1;
    public float Steering = 1;
    public float Grip = 1;
    private Rigidbody rigidBody;
    private float acceleratorValue;
    private float steeringValue;

    // Start is called before the first frame update
    void Start()
    {
        this.rigidBody = this.GetComponent<Rigidbody>();
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
        this.rigidBody.AddForceAtPosition(this.transform.forward * this.acceleratorValue * this.Acceleration, this.FrontWheels.transform.position, ForceMode.Force);
        this.rigidBody.AddForce(this.transform.right * - this.transform.InverseTransformVector(this.rigidBody.velocity).x * this.Grip);
        this.transform.RotateAround(this.BackWheels.transform.position, this.transform.up, this.steeringValue * this.Steering * this.transform.InverseTransformVector(this.rigidBody.velocity).z);
    }
}
