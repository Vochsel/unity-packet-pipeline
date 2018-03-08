using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;

using System.Text;
using System;

public class UPP_Manager : MonoBehaviour {

    public static UPP_Manager MainUPPManager;

    const int PORT = 4000;

    public string RemoteIP;

    UDPSocket twoWaySocket;

    UPP_Component[] trackedComponents;

    // Use this for initialization
    void Start () {
        MainUPPManager = this;

        trackedComponents = FindObjectsOfType<UPP_Component>();

        twoWaySocket = new UDPSocket(RemoteIP, PORT);

        twoWaySocket.ReceivePacketHook += ReceiveMessage;
    }


    void OnApplicationQuit()
    {
        twoWaySocket.Close();
    }

    // Update is called once per frame
    void Update () {
        
    }

    public void SendComponent(UPP_Component a_component, string a_message)
    {
        SendComponent(a_component, Encoding.UTF8.GetBytes(a_message));
    }

    public void SendComponent(UPP_Component a_component, byte[] a_message)
    {
        byte[] buffer = new byte[a_message.Length + 1];
        buffer[0] = (byte)a_component.ID;   //TODO: Fix unsafe case

        Buffer.BlockCopy(a_message, 0, buffer, 1, a_message.Length);

        twoWaySocket.SendPacket(buffer);
    }


    void ReceiveMessage(byte[] a_buffer, IPEndPoint a_remote)
    {
        SendToListeningComponents(a_buffer);
    }

    void SendToListeningComponents(byte[] a_buffer)
    {
        //Get message comp ID
        int compID = a_buffer[0];
        string message = Encoding.UTF8.GetString(a_buffer, 1, a_buffer.Length - 1);
        //Debug.Log("Sending msg: " + message + " - ID: " + compID);

        foreach (UPP_Component comp in trackedComponents)
        {
            if (comp.ID == compID)
                comp.ReceiveData(message);
        }
    }
}
