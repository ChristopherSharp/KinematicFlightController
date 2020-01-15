using UnityEngine;

public class DragFlightControlSystem : FlightControlSystem
{
    protected override void Start()
    {
        //Set drag to 0.5
        _rigidbody.drag = 0.5f;
        _rigidbody.velocity = Vector3.forward * StartVelocity;
    }

    protected override void ApproachLanding()
    {
        CurrentVelocity = _rigidbody.velocity.z;
        var targetDistance = (TargetStop.position - transform.position).z;

        var dragVelocity = CurrentVelocity * (1 - Time.fixedDeltaTime * _rigidbody.drag);

        var next2FrameVelocityRough = dragVelocity + MaxAcceleration * Time.fixedDeltaTime * 2;

        //Rough stopping distance..
        RequiredStoppingDistance = (Mathf.Pow(0, 2) - Mathf.Pow(CurrentVelocity, 2)) / (2 * (MinAcceleration - next2FrameVelocityRough * _rigidbody.drag / 1.5f));

        //Time to de-accelerate to stop!
        if (RequiredStoppingDistance >= targetDistance)
        {
            Output = MinAcceleration;
        }
        //Still a ways out, keep accelerating.
        else
        {
            Output = MaxAcceleration;
        }

        _rigidbody.AddForce(Vector3.forward * Output, ForceMode.Acceleration);
    }
}