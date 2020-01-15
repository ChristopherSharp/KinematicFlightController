using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlightControlSystem : MonoBehaviour
{
    public Transform TargetStop;

    public float StartVelocity = 10;
    public float MaxAcceleration = 5;
    public float MinAcceleration = -3;

    [Header("OutputAcceleration")]
    public float Output;
    public float CurrentVelocity;
    public bool HasLanded;
    public float RequiredStoppingDistance;

    protected Rigidbody _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        //No gravity, constrain to Z only for simplicity.
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }

    protected virtual void Start()
    {
        //Launch!
        _rigidbody.velocity = Vector3.forward * StartVelocity;

        //Set drag to 0
        _rigidbody.drag = 0;
    }

    private void FixedUpdate()
    {
        if (!HasLanded)
        {
            ApproachLanding();           
        }
    }

    protected virtual void ApproachLanding()
    {
        CurrentVelocity = _rigidbody.velocity.z;
        var targetDistance = (TargetStop.position - transform.position).z;

        //I want one frame of fluff here so we forecast out 2 frames.
        var next2FrameVelocity = CurrentVelocity + MaxAcceleration * Time.fixedDeltaTime * 2;

        //The 0 is pointless, but leaving it just so the kinematic assumptions are clear. Vf = 0.
        RequiredStoppingDistance = (Mathf.Pow(0, 2) - Mathf.Pow(next2FrameVelocity, 2)) / (2 * MinAcceleration);

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

    private void OnCollisionEnter(Collision collision)
    {
        if(CurrentVelocity > 0.5f)
        {
            Debug.Log($"That's gonna be expensive.. {name} crashed into landing at <color=#D12626>{CurrentVelocity} m/s</color>");
        }
        else
        {
            Debug.Log($"Boop! :D {name} landed with a comfortable velocity of {CurrentVelocity} m/s");
        }

        HasLanded = true;
    }

    //Draw the required stopping distance. If accelerating be green, if stopping or otherwise be red.
    private void OnDrawGizmos()
    {
        Gizmos.color = Output > 0 ? Color.green : Color.red;

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * RequiredStoppingDistance);
    }

}
