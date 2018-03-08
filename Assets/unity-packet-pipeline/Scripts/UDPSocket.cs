using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public delegate void ReceivePacketDelegate(byte[] a_buffer, IPEndPoint a_remote);

public class UDPSocket {

    Thread receiveThread;

    UdpClient sendSocket = null;
    UdpClient receiveSocket = null;

    public ReceivePacketDelegate ReceivePacketHook;

    bool isListening = false;

    public UDPSocket(string a_remoteAddress = "127.0.0.1", int a_listenPort = 0)
    {
        receiveSocket = new UdpClient(a_listenPort);
        sendSocket = new UdpClient();
        sendSocket.Connect(new IPEndPoint(IPAddress.Parse(a_remoteAddress), a_listenPort));

        Debug.Log("Rece Socket: " + ((IPEndPoint)receiveSocket.Client.LocalEndPoint).Address + ":" + ((IPEndPoint)receiveSocket.Client.LocalEndPoint).Port);
        Debug.Log("Send Socket: " + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Address + ":" + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Port);

        StartListening();
    }

    public void Close()
    {
        if (receiveThread != null && receiveThread.IsAlive)
        {
            isListening = false;
            receiveThread.Abort();
        }
        if (sendSocket != null) sendSocket.Close();
        if (receiveSocket != null) receiveSocket.Close();
    }

    public void SendPacket(byte[] a_buffer)
    {            
        sendSocket.Send(a_buffer, a_buffer.Length);
    }

    public void StartListening()
    {
        Debug.Log("Starting listening");
        if (receiveSocket.Client.LocalEndPoint != null)
        {
            Debug.Log("Listening: " + ((IPEndPoint)receiveSocket.Client.LocalEndPoint).Port);
        }

        receiveThread = new Thread(new ThreadStart(ListenCallback));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        isListening = true;
    }
    
    private void ListenCallback()
    {
        OnListening();
        Debug.Log("Listening");
        while (isListening)
        {
            Debug.Log("Looping");
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

    /* -- Callbacks -- */

    protected virtual void OnListening() { }

    protected virtual void OnReceivePacket(byte[] a_buffer, IPEndPoint a_remote) { ReceivePacketHook(a_buffer, a_remote); }
}
