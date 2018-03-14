﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UnityPacketPipeline
{
    /**
     *  Name: UPP_UDP_TwoWaySocket
     *
     *  Auth: Ben Skiner
     *  Date: 8/03/18
     *  Desc: Handles send and receiving of socket data between two connections
     */
	public class UPP_UDP_TwoWaySocket : UPP_Base_TwoWaySocket
    {
        // -- Internal variables

        // UDP Client to send packets to specified IP and PORT
        UdpClient sendSocket = null;

        // UDP Client to receive packets at specified PORT
        UdpClient receiveSocket = null;

        // -- Constructor

        // Specific constructor to pass address and port
        public UPP_UDP_TwoWaySocket(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000)
        {
            // Open connection
            Open(a_remoteAddress, a_listenPort);
        }

        // -- Connection Functionality

        // Opens two way connection. Sends data to remote ADDRESS and PORT. Listens on PORT
		public override void Open(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000)
        {
			base.Open(a_remoteAddress, a_listenPort);

            // Create clients
            receiveSocket = new UdpClient(a_listenPort);
            sendSocket = new UdpClient();

            // Connect sending socket
            sendSocket.Connect(new IPEndPoint(IPAddress.Parse(a_remoteAddress), a_listenPort));

            // Debug Logs
            Debug.Log("Receiving Socket: " + ((IPEndPoint)receiveSocket.Client.LocalEndPoint).Address + ":" + ((IPEndPoint)receiveSocket.Client.LocalEndPoint).Port);
            Debug.Log("Send Socket: " + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Address + ":" + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Port);

            // Begin listening thread
            StartListening();
        }

        // Correctly closes sockets and stops thread
		public override void Close()
        {
			base.Close ();

            // Stop listening thread
            StopListening();

            // Close sockets
            if (sendSocket != null) sendSocket.Close();
            if (receiveSocket != null) receiveSocket.Close();
        }

        // -- Sending Functionality

        // Send packet
        public override void SendPacket(byte[] a_buffer)
        {
			base.OnSendPacket (a_buffer);

            sendSocket.Send(a_buffer, a_buffer.Length);

            OnSendPacket(a_buffer);
        }

        // -- Listening Functionality

        // Handle listening
		protected override void ListenCallback()
        {
			base.ListenCallback ();

            while (isListening)
            {
                try
                {
                    //IP Of sender... could be anyone
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = receiveSocket.Receive(ref anyIP);

                    //Debug.Log("Rece sock: rece data: " + Encoding.UTF8.GetString(data));

                    OnReceivePacket(data, anyIP);
                    //Thread.Sleep(1);
                }
                catch (Exception err)
                {
                    Debug.Log(err.ToString());
                }
            }
        }

    }

}