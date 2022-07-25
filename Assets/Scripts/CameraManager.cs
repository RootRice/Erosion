using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    List<CameraMode> cameraConstraints;
    CameraMode defaultCamera;

    Transform _transform;
    Vector3 cameraVel;
    float cameraRotProgress;
    float cameraRotVel;

    Transform player;

    [Header("Camera Properties")]
    [SerializeField] Vector3 defaultOffset;
    [SerializeField] float defaultSpeed;
    [SerializeField] float defaultRotationSpeed;


    private void Awake()
    {
        cameraConstraints = new List<CameraMode>();
        defaultCamera = new FollowCamera(defaultOffset, defaultSpeed, defaultRotationSpeed);

        _transform = transform;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void FixedUpdate()
    {
        CameraMode.Parameters cameraParameters;
        int constraintCount = cameraConstraints.Count;

        switch(constraintCount)
        {
            case 0:
                cameraParameters = defaultCamera.GetPositionAndRotation(player.position);
                break;

            default:
                cameraParameters = cameraConstraints[constraintCount - 1].GetPositionAndRotation(player.position);
                break;
        }
        //float speed = cameraParameters.speed;
        //Vector3 pos = Vector3.SmoothDamp(_transform.position, cameraParameters.position, ref cameraVel, speed);
        //cameraRotProgress = Mathf.SmoothDamp(cameraRotProgress, 1f, ref cameraRotVel, speed * speed);
        //Quaternion rot = Quaternion.Slerp(_transform.rotation, cameraParameters.rotation, cameraRotProgress);
        _transform.SetPositionAndRotation(cameraParameters.position, cameraParameters.rotation);
       

    }

    public void SetCameraConstraint(CameraMode constraint)
    {
        defaultCamera.Reset();
        cameraConstraints.Add(constraint);

    }

    public void RemoveCameraConstraint(CameraMode constraint)
    {
        constraint.Reset();
        cameraConstraints.Remove(constraint);
        ResetCameraVelocity();
    }

    private void ResetCameraVelocity()
    {
        cameraRotProgress = 0;
        cameraVel = Vector3.zero;
        cameraRotVel = 0;
    }
}



public interface CameraMode
{
    struct Parameters
    {
        public Vector3 position;
        public Quaternion rotation;
        public float speed;
    }
    enum Modes
    {
        LookAtObject,
        AnchorToPosition,
        Static,
        Follow
    }
    public Parameters GetPositionAndRotation(Vector3 playerPosition);

    public void Reset();
}

public class LookAtCamera : CameraMode
{
    Vector3 objectPos;
    float distToPlayer;
    float heightMod;

    //Camera movement
    float camSpeed;
    float camRotSpeed;
    Transform cameraTransform;
    Vector3 cameraVel;
    float cameraRotVel;
    float cameraRotProgress;
    public LookAtCamera(Vector3 objectPosition, float distanceFromPlayer, float heightAbovePlayer, float cameraSpeed, float cameraRotationSpeed)
    {
        objectPos = objectPosition;
        distToPlayer = distanceFromPlayer;
        heightMod = heightAbovePlayer;

        camSpeed = cameraSpeed;
        camRotSpeed = cameraRotationSpeed;
        cameraTransform = Camera.main.transform;
    }
    public CameraMode.Parameters GetPositionAndRotation(Vector3 playerPosition)
    {
        Vector3 adjustedPlayerPos = playerPosition + Vector3.up * heightMod;
        Vector3 cameraDir = (objectPos - adjustedPlayerPos).normalized;
        Vector3 cameraPos = objectPos + cameraDir * -distToPlayer;
        cameraPos = Vector3.SmoothDamp(cameraTransform.position, cameraPos, ref cameraVel, camSpeed);

        Quaternion cameraRot = Quaternion.LookRotation(objectPos - cameraPos);
        cameraRotProgress = Mathf.SmoothDamp(cameraRotProgress, 1f, ref cameraRotVel, camRotSpeed);
        cameraRot = Quaternion.Slerp(cameraTransform.rotation, cameraRot, cameraRotProgress);

        return new CameraMode.Parameters() { position = cameraPos, rotation = cameraRot, speed = 0 };
    }

    public void Reset()
    {
        cameraVel = Vector3.zero;
        cameraRotVel = 0f;
        cameraRotProgress = 0f;
    }
}

public class AnchorCamera : CameraMode
{
    Vector3 anchorPos;
    float addHeight;

    //Camera movement
    float camSpeed;
    float camRotSpeed;
    Transform cameraTransform;
    Vector3 cameraVel;
    float cameraRotVel;
    float cameraRotProgress;
    public AnchorCamera(Vector3 anchorPosition, float additionalHeight, float cameraSpeed, float cameraRotationSpeed)
    {
        anchorPos = anchorPosition;
        addHeight = additionalHeight;
        camSpeed = cameraSpeed;
        camRotSpeed = cameraRotationSpeed;
        cameraTransform = Camera.main.transform;
    }
    public CameraMode.Parameters GetPositionAndRotation(Vector3 playerPosition)
    {
        Vector3 cameraPos = new Vector3(anchorPos.x, playerPosition.y + addHeight, anchorPos.z);
        Vector3 cameraDir = (playerPosition - cameraPos).normalized;
        Quaternion cameraRot = Quaternion.LookRotation(cameraDir);

        cameraPos = Vector3.SmoothDamp(cameraTransform.position, cameraPos, ref cameraVel, camSpeed);
        cameraRotProgress = Mathf.SmoothDamp(cameraRotProgress, 1f, ref cameraRotVel, camRotSpeed);
        cameraRot = Quaternion.Slerp(cameraTransform.rotation, cameraRot, cameraRotProgress);

        return new CameraMode.Parameters() { position = cameraPos, rotation = cameraRot, speed = 0 };
    }

    public void Reset()
    {
        cameraVel = Vector3.zero;
        cameraRotVel = 0f;
        cameraRotProgress = 0f;
    }
}

public class StaticCamera : CameraMode
{
    Vector3 cameraPos;
    Quaternion lookDir;

    //Camera movement
    float camSpeed;
    float camRotSpeed;
    Transform cameraTransform;
    Vector3 cameraVel;
    float cameraRotVel;
    float cameraRotProgress;
    public StaticCamera(Vector3 cameraPosition, Vector3 lookDirection, float cameraSpeed, float cameraRotationSpeed)
    {
        cameraPos = cameraPosition;
        lookDir = Quaternion.Euler(lookDirection);
        camSpeed = cameraSpeed;
        camRotSpeed = cameraRotationSpeed;
        cameraTransform = Camera.main.transform;
    }
    public CameraMode.Parameters GetPositionAndRotation(Vector3 playerPosition)
    {
        Vector3 pos = Vector3.SmoothDamp(cameraTransform.position, cameraPos, ref cameraVel, camSpeed);
        cameraRotProgress = Mathf.SmoothDamp(cameraRotProgress, 1f, ref cameraRotVel, camRotSpeed);
        Quaternion rot = Quaternion.Slerp(cameraTransform.rotation, lookDir, cameraRotProgress);

        return new CameraMode.Parameters() { position = pos, rotation = rot, speed = 0 };
    }

    public void Reset()
    {
        cameraVel = Vector3.zero;
        cameraRotVel = 0f;
        cameraRotProgress = 0f;
    }
}

public class FollowCamera : CameraMode
{
    Vector3 offset;

    //Camera movement
    float camSpeed;
    float camRotSpeed;
    Transform cameraTransform;
    Vector3 cameraVel;
    float cameraRotVel;
    float cameraRotProgress;
    public FollowCamera(Vector3 cameraOffset, float cameraSpeed, float cameraRotationSpeed)
    {
        offset = cameraOffset;
        camSpeed = cameraSpeed;
        camRotSpeed = cameraRotationSpeed;
        cameraTransform = Camera.main.transform;
    }
    public CameraMode.Parameters GetPositionAndRotation(Vector3 playerPosition)
    {
        Vector3 targetPos = playerPosition + offset;
        Vector3 cameraDir = (playerPosition - targetPos).normalized;
        Quaternion cameraAngle = Quaternion.LookRotation(cameraDir);

        Vector3 pos = Vector3.SmoothDamp(cameraTransform.position, targetPos, ref cameraVel, camSpeed);
        cameraRotProgress = Mathf.SmoothDamp(cameraRotProgress, 1f, ref cameraRotVel, camRotSpeed);
        Quaternion rot = Quaternion.Slerp(cameraTransform.rotation, cameraAngle, cameraRotProgress);

        return new CameraMode.Parameters() { position = pos, rotation = rot, speed = 0 };
    }

    public void Reset()
    {
        cameraVel = Vector3.zero;
        cameraRotVel = 0f;
        cameraRotProgress = 0f;
    }
}