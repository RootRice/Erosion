using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour, IMovePart
{

    private Rigidbody _rb;
    private MoveController _movement;
    private InputController _input;
    private ForceTracker _forces;

    [SerializeField] private float _maxBlockTime;
    [SerializeField] private float _blockDescent;

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
        // _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        if (!_input.Grounded)
        {
            _rb.velocity = Vector3.zero;
        }
        _input.InputMultiplier = 1;
        Vector3 totalForce;
        for (float timer = 0; timer < _maxBlockTime && _input.BlockHeld; timer += Time.deltaTime)
        {
            totalForce = _forces.ContainsKey("Block") ? _forces.TotalForce - _forces["Block"] : _forces.TotalForce;
            _forces["Block"] = new Vector3(-totalForce.x, -totalForce.y - _blockDescent, -totalForce.z);
            //_rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            _rb.velocity = new Vector3(0, 0 + _blockDescent, 0);
            yield return new WaitForFixedUpdate();
        }
        _forces.Remove("Block");
        _input.InputMultiplier = 1;
    }

    //public IEnumerator Run()
    //{
    //    print("Ran");
    //    yield return null;
    //}

    public void Stop()
    {
        StopAllCoroutines();
        _input.BlockHeld = false;
        _forces.Remove("Block");
        _input.InputMultiplier = 1;
    }
}



