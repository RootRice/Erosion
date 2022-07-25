using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraVolume))] [System.Serializable]
public class CameraParameterEditor : Editor
{
    CameraVolume volume;
    [SerializeField, HideInInspector] float fValue1;
    [SerializeField, HideInInspector] float fValue2;
    [SerializeField, HideInInspector] float speed;
    [SerializeField, HideInInspector] float rotSpeed;
    [SerializeField, HideInInspector] Vector3 vValue1;
    [SerializeField, HideInInspector] Vector3 vValue2;
    [SerializeField, HideInInspector] Transform t;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        switch (volume.cameraMode)
        {
            case CameraMode.Modes.AnchorToPosition:
                vValue1 = EditorGUILayout.Vector3Field("Anchor Position", vValue1);
                fValue1 = EditorGUILayout.FloatField("Height Over Player", fValue1);
                GUILayout.BeginHorizontal();
                GUILayout.Space(Screen.width / 2);
                if (GUILayout.Button("Sync Scene Camera", new GUILayoutOption[] { GUILayout.Width(Screen.width / 3) })) { vValue1 = SceneView.lastActiveSceneView.camera.transform.position; }
                GUILayout.EndHorizontal();
                break;

            case CameraMode.Modes.LookAtObject:
                fValue1 = EditorGUILayout.FloatField("Distance from player", fValue1);
                fValue2 = EditorGUILayout.FloatField("Height above player", fValue2);
                t = (Transform)EditorGUILayout.ObjectField("Object To Look At", t, typeof(Transform), true, new GUILayoutOption[] { });
                vValue2 = EditorGUILayout.Vector3Field("Camera Offset", vValue2);
                if (t == null) break;
                volume.tVal = t;
                vValue1 = t.position + vValue2;
                break;

            case CameraMode.Modes.Static:
                vValue1 = EditorGUILayout.Vector3Field("Anchor Position", vValue1);
                vValue2 = EditorGUILayout.Vector3Field("Rotation", vValue2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(Screen.width/2);
                if (GUILayout.Button("Sync Scene Camera", new GUILayoutOption[] { GUILayout.Width(Screen.width/3) }))
                    { vValue1 = SceneView.lastActiveSceneView.camera.transform.position; vValue2 = SceneView.lastActiveSceneView.rotation.eulerAngles; }
                GUILayout.EndHorizontal();
                break;

            case CameraMode.Modes.Follow:
                vValue1 = EditorGUILayout.Vector3Field("Anchor Position", vValue1);
                break;

            default:
                break;
        }
        speed = EditorGUILayout.FloatField("Camera Speed", speed);
        rotSpeed = EditorGUILayout.FloatField("Camera Rotation Speed", rotSpeed);

        volume.vVal1 = vValue1;
        volume.vVal2 = vValue2;
        volume.fVal1 = fValue1;
        volume.fVal2 = fValue2;
        volume.speed = speed;
        volume.rotSpeed = rotSpeed;
    }

    private void OnEnable()
    {
        volume = (CameraVolume)target;
        vValue1 = volume.vVal1;
        vValue2 = volume.vVal2;
        fValue1 = volume.fVal1;
        fValue2 = volume.fVal2;
        speed = volume.speed;
        rotSpeed = volume.rotSpeed;
        t = volume.tVal;
    }
}
