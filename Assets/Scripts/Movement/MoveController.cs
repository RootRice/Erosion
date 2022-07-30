using System.Collections;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    private Rigidbody _rb;
    private JumpController _jump;
    private InputController _input;

    public ForceTracker Forces = new ForceTracker();

    // Move vars
    [SerializeField, Range(0,20)] private float _speed = 5f;
    [SerializeField, Range(0, 200)] private float _turnSpeed = 180f;
    // gravity vars
    [SerializeField] public float Gravity = 9.81f;

    [SerializeField, Range(0f, 100f)] float _drag = 5f;

    [SerializeField] private Vector3 vel;
    private bool _grounded;

    private Vector3 _moveForce;
    public Vector3 LastInput;

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
        Forces["Move"] = _moveForce * _input.InputMultiplier;
        Vector3 adjustedForce = Vector3.ProjectOnPlane(Forces.TotalForce, _input.ContactNormal) + Forces["Jump"] + Forces["Dodge"] + Forces["Gravity"] + Forces["Hang"];

        _rb.velocity += adjustedForce;
        Look(LastInput);

        vel = _rb.velocity;

    }

    public void Move(Vector2 input)
    {
        Vector3 vec3Input = new Vector3(input.x, 0, input.y);
        Vector3 rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up) * vec3Input;
        LastInput = rotation != Vector3.zero ? rotation : LastInput;
        
        _moveForce = rotation * vec3Input.magnitude * _speed;
    }

    public void Look(Vector3 input)
    {
        input *= _input.InputMultiplier;
        if (input != Vector3.zero)
        {
            var rotation = Quaternion.LookRotation(input, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _turnSpeed * Time.deltaTime);
        }
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

    
}
