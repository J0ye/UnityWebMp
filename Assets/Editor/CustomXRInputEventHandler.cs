using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(XRInputEventHandler), true)]
public class CustomXRInputEventHandler : Editor
{
    public override void OnInspectorGUI()
    {
        XRInputEventHandler script = (XRInputEventHandler)target;

        //EditorGUILayout.LabelField("XR Devices", script.inputDevices.Count.ToString());

        DrawDefaultInspector();

    }
}