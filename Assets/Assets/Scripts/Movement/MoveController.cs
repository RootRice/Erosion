using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    private Rigidbody _rb;
    private JumpController _jump;
    private InputController _input;

    public ForceTracker Forces = new ForceTracker();

    // Move vars
    [SerializeField, Range(0,20)] private float _speed = 5f;
    [SerializeField, Range(0, 20)] private float _turnSpeed = 5f;
    // gravity vars
    [SerializeField] public float Gravity = 9.81f;

    [SerializeField, Range(0f, 100f)] float _drag = 5f;

    [SerializeField] private Vector3 vel;
    private bool _grounded;

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _jump = gameObject.GetComponent<JumpController>();
        _input = gameObject.GetComponent<InputController>();
        _grounded = _input.Grounded;

        // Initialise gravity
        Forces["Gravity"] = new Vector3(0, -(Gravity * Time.deltaTime), 0);
        // Initialise drag
        Forces["Drag"] = Vector3.zero;
        //Initialise move
        Forces["Move"] = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        _grounded = _input.Grounded;
        Forces["Drag"] = new Vector3(-(_rb.velocity.x * (1 - (100 - _drag) / 100)), Forces["Drag"].y, -(_rb.velocity.z * (1 - (100 - _drag) / 100)));
        Forces["Gravity"] = _grounded ? Vector3.zero : new Vector3(0, -(Gravity * Time.deltaTime), 0);
        // var adjustedForce = AdjustVelocityToSlope(Forces.TotalForce);
        // _rb.velocity += adjustedForce;
        _rb.velocity += Forces.TotalForce;
        vel = _rb.velocity;
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - _input.ContactNormal * Vector3.Dot(vector, _input.ContactNormal);
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        {
            var ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
            {
                var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                var adjustedVelocity = slopeRotation * velocity;

                if (adjustedVelocity.y < 0)
                {
                    return adjustedVelocity;
                }
            }
            return velocity;
        }
        
    }

    public void Move(Vector2 input)
    {
        Vector3 vec3Input = new Vector3(input.x, 0, input.y);
        Look(vec3Input);

        Forces["Move"] = transform.forward * vec3Input.magnitude * _speed;
    }

    public IEnumerator Slow(Vector2 input)
    {
        while(input == Vector2.zero && (_rb.velocity.x != 0 || _rb.velocity.z != 0))
        {
            Forces["Drag"] = new Vector3(-(_rb.velocity.x * (1-(100 - _drag) / 100)), 0, -(_rb.velocity.z * (1 - (100 - _drag) / 100)));
            yield return new WaitForFixedUpdate();
        }
        Forces.Remove("Drag");
    }

    public void Look(Vector3 input)
    {
        if (input != Vector3.zero)
        {
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

            var skewedInput = matrix.MultiplyPoint3x4(input);

            var rotation = Quaternion.LookRotation(skewedInput, Vector3.up);

            // transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _turnSpeed * Time.deltaTime);
            transform.rotation = rotation;
        }
    }
}
