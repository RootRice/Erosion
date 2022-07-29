using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float _camDistance = 10;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(-_camDistance, 0.8f * _camDistance, -_camDistance);
    }
}
