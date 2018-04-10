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
    public void OnEditorUpdate()
    {
        Debug.Log("UPDATING");
        // This will only get called 10 times per second.
        Repaint();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        UPP_Manager uppm = (UPP_Manager)target;
        GUIStyle curStyle = uppm.ManagerStatus < UPP_ManagerStatus.CLOSED ? styleGreen : styleRed;
        curStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Status: " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(uppm.ManagerStatus.ToString().ToLower()), curStyle);

        // -- Sockets

        GUIStyle bold = new GUIStyle();
        bold.fontStyle = FontStyle.Bold;

        EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Send Socket", bold);
            EditorGUILayout.LabelField("Status: " + uppm.SendSocket);
            if(uppm.GetTwoWaySocket != null)
                EditorGUILayout.LabelField("Sent: " + uppm.GetTwoWaySocket.PacketsSent);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Receive Socket", bold);
            EditorGUILayout.LabelField("Status: " + uppm.ReceiveSocket);
            if (uppm.GetTwoWaySocket != null)
                EditorGUILayout.LabelField("Received: " + uppm.GetTwoWaySocket.PacketsReceived);

        EditorGUILayout.EndVertical();



        EditorGUILayout.Separator();

        // -- Thread status

        if(uppm.GetTwoWaySocket != null)
            EditorGUILayout.LabelField("Listen Thread Status " + uppm.GetTwoWaySocket.IsListening);
        else
            EditorGUILayout.LabelField("Not Listening");


        EditorUtility.SetDirty(target);
    }
}
