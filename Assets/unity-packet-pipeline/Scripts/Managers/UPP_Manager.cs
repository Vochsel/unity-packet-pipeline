﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;

using System.Text;
using System;


namespace UnityPacketPipeline
{
    /**
    *  Name: UPP_Manager
    *
    *  Auth: Ben Skiner
    *  Date: 8/03/18
    *  Desc: Unity component to manage all UPP_Components. Handles send and receive functionality.
    */
    public class UPP_Manager : MonoBehaviour
    {

        // -- Public Static Variables

        // Singleton instance
        public static UPP_Manager MainUPPManager;

        // -- public Manager Settings

        // Remote IP of upp connection
        public string RemoteIP = "127.0.0.1";

        // Port to both send packets to and listen for packets on
        public int Port = 4000;

        // -- Private Member Variables

        // UDP Socket used for communication
        UPP_UDP_TwoWaySocket twoWaySocket;

        // All components in the scene to track
        UPP_Component[] trackedComponents;

        // -- Unity Events

        // Link and register manager functionality
        void Start()
        {
            // Store singleton instance
            MainUPPManager = this;

            // Find all scene UPP_Components
            trackedComponents = FindObjectsOfType<UPP_Component>();

            // Setup network connection
            twoWaySocket = new UPP_UDP_TwoWaySocket(RemoteIP, Port);

            // Register callback on packet received
            twoWaySocket.ReceivePacketHook += ReceiveMessage;
        }

        // Clean up network connection
        void OnApplicationQuit()
        {
            twoWaySocket.Close();
        }

        // -- Public Utility Functions

        // Send packet with prepended component ID in buffer
        public void SendComponent(UPP_Component a_component, byte[] a_message)
        {
            // Create new buffer
            byte[] buffer = new byte[a_message.Length + 1];

            // Prepend ID
            buffer[0] = (byte)a_component.ID;   //TODO: Fix unsafe case

            // Copy buffer contents
            Buffer.BlockCopy(a_message, 0, buffer, 1, a_message.Length);

            // Send packet
            twoWaySocket.SendPacket(buffer);
        }

        // Send component string message 
        public void SendComponent(UPP_Component a_component, string a_message)
        {
            //Encode string to UTF8 bytes
            SendComponent(a_component, Encoding.UTF8.GetBytes(a_message));
        }

        // Receive message callback
        void ReceiveMessage(byte[] a_buffer, IPEndPoint a_remote)
        {
            SendToListeningComponents(a_buffer);
        }

        // Send buffer to listening components
        // TODO: Convert trackedComponents to Map of arrays with ID as key
        void SendToListeningComponents(byte[] a_buffer)
        {
            // Get message comp ID
            int compID = a_buffer[0];
            string message = Encoding.UTF8.GetString(a_buffer, 1, a_buffer.Length - 1);

            foreach (UPP_Component comp in trackedComponents)
            {
                // If ID matches trigger callback
                if (comp.ID == compID)
                    comp.ReceiveData(message);
            }
        }
    }
}