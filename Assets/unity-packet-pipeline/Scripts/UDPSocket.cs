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

    public UDPSocket(bool a_shouldListen, string a_address = "localhost", uint a_port = 0)
    {
        if (a_shouldListen)
        {
            StartListening();
        }
        sendSocket = new UdpClient();
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

    public void SendPacket(byte[] a_buffer, IPEndPoint a_remote)
    {
        if(receiveSocket != null) {
            receiveSocket.Send(a_buffer, a_buffer.Length, a_remote);
            Debug.Log("Receive sock: sent data");
        }
        else if(sendSocket != null) {
            
            sendSocket.Send(a_buffer, a_buffer.Length, a_remote);
           // Debug.Log("Send sock: sent data: " + Encoding.UTF8.GetString(a_buffer));
        }
    }

    public void BroadcastPacket(byte[] a_buffer, int a_port) 
    {
        IPEndPoint broadcastIP = new IPEndPoint(IPAddress.Broadcast, 0);
        SendPacket(a_buffer, broadcastIP);
    }

    public void StartListening()
    {
        Debug.Log("Starting listening");
        receiveSocket = new UdpClient(3000);
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
           // Debug.Log("Looping");
            try
            {
                
                if (receiveSocket.Client.LocalEndPoint == null) continue;
                

                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = receiveSocket.Receive(ref anyIP);
                
               // Debug.Log("Rece sock: rece data: " + Encoding.UTF8.GetString(data));
                
                OnReceivePacket(data, anyIP);
                //Thread.Sleep(10);
            }
            catch (SocketException err)
            {
                Debug.Log(err.ToString());
            }
        }
    }

    /* -- Callbacks -- */

    protected virtual void OnListening() { }

    protected virtual void OnReceivePacket(byte[] a_buffer, IPEndPoint a_remote) { ReceivePacketHook(a_buffer, a_remote); }
}
