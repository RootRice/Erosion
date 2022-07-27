using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectAction : MonoBehaviour, EventAction
{
    private Vector3 startPos;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float speed;

    private Vector3 currentTarget;
    void Awake()
    {
        startPos = transform.position;
        currentTarget = startPos;
    }
    public void ResetAction()
    {
        
    }
    public void TriggerAction()
    {
        currentTarget = targetPosition;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
    }
}
