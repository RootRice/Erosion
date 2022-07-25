using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVolume : MonoBehaviour
{
    public CameraMode.Modes cameraMode;
    [HideInInspector] public Vector3 vVal1;
    [HideInInspector] public Vector3 vVal2;
    [HideInInspector] public float fVal1;
    [HideInInspector] public float fVal2;
    [HideInInspector] public float speed;
    [HideInInspector] public float rotSpeed;
    [HideInInspector] public Transform tVal;
    CameraMode myMode;

    CameraManager manager;
    Collider player;


    void Awake()
    {
        switch(cameraMode)
        {
            case CameraMode.Modes.AnchorToPosition:
                myMode = new AnchorCamera(vVal1, fVal1, speed, rotSpeed);
                break;

            case CameraMode.Modes.LookAtObject:
                myMode = new LookAtCamera(vVal1, fVal1, fVal2, speed, rotSpeed);
                break;

            case CameraMode.Modes.Static:
                myMode = new StaticCamera(vVal1, vVal2, speed, rotSpeed);
                break;

            case CameraMode.Modes.Follow:
                myMode = new FollowCamera(vVal1, speed, rotSpeed);
                break;

            default:
                break;
        }

        manager = Camera.main.GetComponent<CameraManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == player)
            manager.SetCameraConstraint(myMode);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other == player)
            manager.RemoveCameraConstraint(myMode);
    }
}
