using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityPacketPipeline;
using System.Net;
using System.Net.Sockets;
using System;

[CustomEditor(typeof(UPP_Manager))]
[CanEditMultipleObjects]
public class UPP_Manager_Editor : Editor {

    GUIStyle styleGreen = new GUIStyle();
    GUIStyle styleRed = new GUIStyle();

    string localIP;

    void OnEnable()
    {
        //lookAtPoint = serializedObject.FindProperty("lookAtPoint");

        styleGreen.normal.textColor = new Color(0.1f, 0.5f, 0.05f);
        styleRed.normal.textColor = new Color(0.6f, 0.1f, 0.05f);

        localIP = GetLocalIPAddress();
    }
    public void OnEditorUpdate()
    {
        Debug.Log("UPDATING");
        // This will only get called 10 times per second.
        Repaint();
    }
    public override void OnInspectorGUI()
    {
        GUIStyle bold = new GUIStyle();
        bold.fontStyle = FontStyle.Bold;

        base.OnInspectorGUI();
        UPP_Manager uppm = (UPP_Manager)target;

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField(uppm.Protocol + " Connection Details", bold);
        EditorGUILayout.LabelField("Local IP: " + localIP);

        EditorGUILayout.Separator();

        GUIStyle curStyle = uppm.ManagerStatus < UPP_ManagerStatus.CLOSED ? styleGreen : styleRed;
        curStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Status: " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(uppm.ManagerStatus.ToString().ToLower()), curStyle);
        
        // -- Sockets


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

        EditorGUILayout.Separator();


        if (uppm.ConnectedComponents != null && uppm.ConnectedComponents.Count > 0)
        {
            EditorGUILayout.LabelField("Connected components - " + uppm.ConnectedComponents.Count, bold);
            foreach (UPP_Component uppc in uppm.ConnectedComponents)
            {
                EditorGUILayout.LabelField("[" + uppc.name + "] - " + uppc.GetType().ToString());
            }
        }
        else
        {
            EditorGUILayout.LabelField("Connected components", bold);
            EditorGUILayout.LabelField("No connected components");
        }



        EditorUtility.SetDirty(target);
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
