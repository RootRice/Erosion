using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeController : MonoBehaviour, IMovePart
{

    private Rigidbody _rb;
    private MoveController _movement;
    private InputController _input;

    public float _maxTime;
    [SerializeField, Range(0.1f, 1f)] private float _minHeldTime;

    [SerializeField] float _multiplier = 1;

    private bool _held = true;
    public float _timeHeld;

    [SerializeField] private AnimationCurve _horizontalCurve;
    [SerializeField] private AnimationCurve _verticalCurve;

    private ForceTracker _forces;

    [SerializeField] private AnimationCurve _inputCurve;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _movement = gameObject.GetComponent<MoveController>();
        _input = gameObject.GetComponent<InputController>();
        _forces = _movement.Forces;
    }

    public IEnumerator Run()
    {
        //_rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        if(!_input.Grounded)
        {
            _rb.velocity = Vector3.zero;
        }
        var horizontalTime = _horizontalCurve.keys[_horizontalCurve.length - 1].time;
        var verticalTime = _horizontalCurve.keys[_horizontalCurve.length - 1].time;
        _maxTime = Mathf.Max(horizontalTime, verticalTime);
        for (float timer = 0; timer < _maxTime; timer += Time.deltaTime)
        {
            _input.InputMultiplier = _inputCurve.Evaluate(timer / _inputCurve.keys[_inputCurve.length - 1].time);
            Vector3 dodgeVelocity = _movement.LastInput * _horizontalCurve.Evaluate(timer);
            dodgeVelocity = new Vector3(dodgeVelocity.x, _verticalCurve.Evaluate(timer), dodgeVelocity.z);
            _forces["Dodge"] = dodgeVelocity;
            yield return new WaitForFixedUpdate();
        }
        _forces.Remove("Dodge");
        _input.InputMultiplier = 1f;
    }

    public void Stop()
    {
        StopAllCoroutines();
        _forces.Remove("Dodge");
        _input.InputMultiplier = 1f;
    }
}



