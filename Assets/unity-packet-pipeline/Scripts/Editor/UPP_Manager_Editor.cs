using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityPacketPipeline;

[CustomEditor(typeof(UPP_Manager))]
[CanEditMultipleObjects]
public class UPP_Manager_Editor : Editor {

    GUIStyle styleGreen = new GUIStyle();
    GUIStyle styleRed = new GUIStyle();

    void OnEnable()
    {
        //lookAtPoint = serializedObject.FindProperty("lookAtPoint");

        styleGreen.normal.textColor = new Color(0.1f, 0.5f, 0.05f);
        styleRed.normal.textColor = new Color(0.6f, 0.1f, 0.05f);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();

        UPP_Manager uppm = (UPP_Manager)target;
        GUIStyle curStyle = uppm.ManagerStatus < UPP_ManagerStatus.CLOSED ? styleGreen : styleRed;
        curStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Status: " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(uppm.ManagerStatus.ToString().ToLower()), curStyle);

        EditorGUILayout.LabelField("Send Socket: " + uppm.SendSocket);
        EditorGUILayout.LabelField("Receive Socket: " + uppm.ReceiveSocket);

        EditorGUILayout.Separator();
        if(uppm.GetTwoWaySocket != null)
            EditorGUILayout.LabelField("Listen Thread Status " + uppm.GetTwoWaySocket.IsListening);
        else
            EditorGUILayout.LabelField("Not Listening");

    }
}
