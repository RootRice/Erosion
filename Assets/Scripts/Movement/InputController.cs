using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private PlayerControls _controls;

    private MoveController _movement;
    private JumpController _jump;
    private DodgeController _dodge;
    private BlockController _block;
    private Rigidbody _rb;
    [SerializeField] public bool Grounded;
    private bool _hasAirMove, _hasBlock;

    public bool JumpHeld, BlockHeld;
    [SerializeField] private float _drag;

    [SerializeField, Range(0f, 90f)] private float _maxGroundAngle;
    private float _minGroundDotProduct;
    public Vector3 ContactNormal;

    public float InputMultiplier = 1;

    private Coroutine _jumpCo;

    public int StepsSinceLastGrounded, stepsSinceLastJump;
    [SerializeField, Min(0f)] float probeDistance = 1f;

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Awake()
    {
        _controls = new PlayerControls();
        OnValidate();
    }

    void Start()
    {
        _movement = gameObject.GetComponent<MoveController>();
        _jump = gameObject.GetComponent<JumpController>();
        _dodge = gameObject.GetComponent<DodgeController>();
        _block = gameObject.GetComponent<BlockController>();
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateState();
        Grounded = false;
    }

    private void UpdateState()
    {
        StepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        if (Grounded || SnapToGround())
        {
            StepsSinceLastGrounded = 0;
            _hasAirMove = true;
            _hasBlock = true;
        }
        else
        {
            ContactNormal = Vector3.up;
        }
    }

    private bool SnapToGround()
    {
        if (StepsSinceLastGrounded > 3 || stepsSinceLastJump <= 5 || _movement.Forces.TotalForce.y > 0)
        {
            return false;
        }

        if (!Physics.Raycast(_rb.position, Vector3.down, out RaycastHit hit, probeDistance))
        {
            return false;
        }

        if (hit.normal.y < _minGroundDotProduct)
        {
            return false;
        }

        ContactNormal = hit.normal;
        float speed = _rb.velocity.magnitude;
        float dot = Vector3.Dot(_rb.velocity, hit.normal);
        if (dot > 0f)
        {
            _rb.velocity = (_rb.velocity - hit.normal * dot).normalized * speed;
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= _minGroundDotProduct)
            {
                Grounded = true;
                ContactNormal = normal;
            }
        }
    }

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(_maxGroundAngle * Mathf.Deg2Rad);
    }

    public void Move(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            default:
                _movement.Move(context.ReadValue<Vector2>());
                break;
        }
        
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if ((Grounded || _hasAirMove || context.phase == InputActionPhase.Canceled) && !_movement.Forces.ContainsKey("Dodge"))
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Debug.Log("Jump Started");
                    JumpHeld = true;
                    break;
                case InputActionPhase.Performed:
                    Debug.Log("Jump Performed");
                    if (!Grounded)
                    {
                        _hasAirMove = false;
                    }
                    stepsSinceLastJump = 0;
                    _block.Stop();
                    _jumpCo = StartCoroutine(_jump.Run());
                    JumpHeld = true;
                    break;
                case InputActionPhase.Canceled:
                    Debug.Log("Jump Cancelled");
                    JumpHeld = false;
                    break;
                default:
                    JumpHeld = false;
                    break;
            }
        }
    }

    public void Dodge(InputAction.CallbackContext context)
    {
        if (Grounded || _hasAirMove || context.phase == InputActionPhase.Canceled)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Debug.Log("Dodge Started");
                    break;
                case InputActionPhase.Performed:
                    Debug.Log("Dodge Performed");
                    if (!Grounded)
                    {
                        _hasAirMove = false;
                    }
                    stepsSinceLastJump = 0;
                    _jump.Stop();
                    _block.Stop();
                    StartCoroutine(_dodge.Run());
                    break;
                case InputActionPhase.Canceled:
                    Debug.Log("Dodge Cancelled");
                    break;
            }
        }
    }

    public void Block(InputAction.CallbackContext context)
    {
        if ((Grounded || _hasBlock || context.phase == InputActionPhase.Canceled))
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Debug.Log("Block Started");
                    BlockHeld = true;
                    break;
                case InputActionPhase.Performed:
                    Debug.Log("Block Perfomed");
                    BlockHeld = true;
                    _hasBlock = false;
                    _jump.Stop();
                    _dodge.Stop();
                    StartCoroutine(_block.Run());
                    break;
                case InputActionPhase.Canceled:
                    BlockHeld = false;
                    break;
                default:
                    break;
            }
        }
    }

    public void StopAll()
    {
        _jump.Stop();
        _dodge.Stop();
        _block.Stop();
    }


}
