using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour, IMovePart
{

    private Rigidbody _rb;
    private MoveController _movement;
    private InputController _input;
    private ForceTracker _forces;

    public float _maxTime;
    [SerializeField, Range(0.1f, 1f)] private float _minHeldTime;

    [SerializeField] float _multiplier = 1;

    private bool _held = true;
    public float _timeHeld;

    [SerializeField]  private AnimationCurve _curve;


    [SerializeField]  private float _hangTime = 1.0f;
    private float _hangDuration;
    private bool _hanging = false;
    [SerializeField] private float _hangDescentRate;

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
        _maxTime = _curve.keys[_curve.length - 1].time;
        _hangDuration = 0;
        _hanging = false;
        // _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        if (!_input.Grounded)
        {
            _rb.velocity = Vector3.zero;
        }
        yield return StartCoroutine(Ascend());
        yield return StartCoroutine(Hang());
    }

    IEnumerator Ascend()
    {
        var maxTimer = _maxTime;
        for (float timer = 0; timer  < maxTimer; timer += Time.deltaTime) {
            if(!_input.JumpHeld && maxTimer == _maxTime)
            {
                maxTimer = timer < _minHeldTime ? _minHeldTime : timer + 0.15f;
            }
            _forces["Jump"] = new Vector3(0, _curve.Evaluate(timer) * _multiplier, 0);
            yield return new WaitForFixedUpdate();
        }
        _forces.Remove("Jump");
       
    }

    IEnumerator Hang()
    {
        while (!_hanging)
        {
            if (_rb.velocity.y <= 0.3 && _rb.velocity.y >= -0.3)
            {
                _hanging = true;
            }
            yield return new WaitForFixedUpdate();
        }

        while (_hanging)
        {
            if (_forces.ContainsKey("Jump") || _hangDuration >= _hangTime)
            {
                _hangDuration = 0;
                _hanging = false;
                _forces.Remove("Hang");
                yield break;
            }
            else
            {
                _hangDuration += Time.deltaTime;
                _forces.Update();
                Vector3 totalForce;
                totalForce = _forces.ContainsKey("Hang") ? _forces.TotalForce - _forces["Hang"] : _forces.TotalForce;
                _forces["Hang"] = new Vector3(0, -totalForce.y - _hangDescentRate, 0);
            }
            yield return new WaitForFixedUpdate();
        }
    }



    public void Stop()
    {
        StopAllCoroutines();
        _forces.Remove("Jump");
        _forces.Remove("Hang");
    }
}


