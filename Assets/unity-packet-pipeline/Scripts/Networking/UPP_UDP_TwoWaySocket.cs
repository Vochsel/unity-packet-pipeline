using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UnityPacketPipeline
{
    // Delegate on packet received
    public delegate void ReceivePacketDelegate(byte[] a_buffer, IPEndPoint a_remote);

    /**
     *  Name: UPP_UDP_TwoWaySocket
     *
     *  Auth: Ben Skiner
     *  Date: 8/03/18
     *  Desc: Handles send and receiving of socket data between two connections
     */
    public class UPP_UDP_TwoWaySocket
    {
        // -- Internal variables
        Thread receiveThread;

        // UDP Client to send packets to specified IP and PORT
        UdpClient sendSocket = null;

        // UDP Client to receive packets at specified PORT
        UdpClient receiveSocket = null;

        // Instance of delegate fired when packet received
        public ReceivePacketDelegate ReceivePacketHook;

        // Bool which controls if the connection is listening
        bool isListening = false;

        // -- Constructor

        // Specific constructor to pass address and port
        public UPP_UDP_TwoWaySocket(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000)
        {
            // Open connection
            Open(a_remoteAddress, a_listenPort);
        }

        // -- Connection Functionality

        // Opens two way connection. Sends data to remote ADDRESS and PORT. Listens on PORT
        public void Open(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000)
        {
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
        public void Close()
        {
            // Stop listening thread
            StopListening();

            // Close sockets
            if (sendSocket != null) sendSocket.Close();
            if (receiveSocket != null) receiveSocket.Close();
        }

        // -- Sending Functionality

        // Send packet
        public void SendPacket(byte[] a_buffer)
        {
            sendSocket.Send(a_buffer, a_buffer.Length);

            OnSendPacket(a_buffer);
        }

        // -- Listening Functionality

        // Handle thread startup
        private void StartListening()
        {
            Debug.Log("Starting listening");

            // Create thread
            receiveThread = new Thread(new ThreadStart(ListenCallback));
            receiveThread.IsBackground = true;

            // Start thread
            receiveThread.Start();
            isListening = true;
        }

        // Handle thread closing
        private void StopListening()
        {
            if (receiveThread != null && receiveThread.IsAlive)
            {
                isListening = false;
                receiveThread.Abort();
            }
        }

        // Handle listening
        private void ListenCallback()
        {
            OnListening();
            Debug.Log("Listening");
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

        // -- Callbacks

        // Called when connection starts listening
        protected virtual void OnListening() { }

        // Called when connection sends packet
        protected virtual void OnSendPacket(byte[] a_buffer) { }

        // Called when connection receives packet
        protected virtual void OnReceivePacket(byte[] a_buffer, IPEndPoint a_remote) { ReceivePacketHook(a_buffer, a_remote); }
    }

}