using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;

namespace UnityPacketPipeline
{
	public class UPP_TCP_TwoWaySocket : UPP_Base_TwoWaySocket
	{
		// -- Internal variables

		// UDP Client to send packets to specified IP and PORT
		TcpClient sendSocket = null;

		// UDP Client to receive packets at specified PORT
		TcpListener receiveSocket = null;

		NetworkStream sendStream, receiveStream;

		// -- Constructor

		// Specific constructor to pass address and port
		public UPP_TCP_TwoWaySocket(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000)
		{
			// Open connection
			Open(a_remoteAddress, a_listenPort);
		}

		// -- Connection Functionality

		// Opens two way connection. Sends data to remote ADDRESS and PORT. Listens on PORT
		public override void Open(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000)
		{
			base.Open(a_remoteAddress, a_listenPort);

			try {
				// Create clients
				OpenReceiveSocket(a_remoteAddress, a_listenPort);

				StartListening();

				OpenSendSocketAsync(a_remoteAddress, a_listenPort);
			} catch(SocketException e) {
				Debug.Log ("Could not create sockets");
				Debug.Log (e);
			}
			// Connect sending socket
			//sendSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3001));

			// Debug Logs
			Debug.Log("Receiving Socket: " + ((IPEndPoint)receiveSocket.LocalEndpoint).Address + ":" + ((IPEndPoint)receiveSocket.LocalEndpoint).Port);
			Debug.Log("Send Socket: " + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Address + ":" + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Port);

			// Begin listening thread

		}

		// Correctly closes sockets and stops thread
		public override void Close()
		{
			base.Close ();

			// Stop listening thread
			StopListening();

			// Close sockets
			CloseSendSocket();
			CloseReceiveSocket ();
		}


		// -- Socket Lifecycle 

		protected override void OpenSendSocket(string a_remoteAddress, int a_listenPort) {
			base.OpenSendSocket (a_remoteAddress, a_listenPort);

			sendSocket = new TcpClient ();
			sendSocket.Connect(new IPEndPoint(IPAddress.Parse(a_remoteAddress), a_listenPort));

			sendStream = sendSocket.GetStream();
		}

		protected override bool OpenSendSocketAsync(string a_remoteAddress, int a_listenPort) {

			sendSocket = new TcpClient ();


			/*sendSocket.BeginConnect(a_remoteAddress, a_listenPort, asyncResult =>
			{
				sendSocket.EndConnect(asyncResult);
				Debug.Log(asyncResult.ToString());

				Debug.Log("Socket connected");
					if(((IAsyncResult)asyncResult).IsCompleted)
						sendStream = sendSocket.GetStream();
			}, null);*/

			var result = sendSocket.BeginConnect(a_remoteAddress, a_listenPort, null, null);
			var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3), true);

			if ( sendSocket.Connected )
			{
				sendSocket.EndConnect( result );
				sendStream = sendSocket.GetStream();
				Debug.Log("Send Socket: " + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Address + ":" + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Port);
			}
			else 
			{
				// NOTE, MUST CLOSE THE SOCKET

				sendSocket.Close();
				Debug.Log ("Failed to connect!");
				//throw new ApplicationException("Failed to connect server.");
			}
				
			return true;
		}

		protected override void OpenReceiveSocket(string a_remoteAddress, int a_listenPort) {
			base.OpenReceiveSocket (a_remoteAddress, a_listenPort);

			receiveSocket = new TcpListener(IPAddress.Any, a_listenPort);
			receiveSocket.Start();

			Debug.Log("Receiving Socket: " + ((IPEndPoint)receiveSocket.LocalEndpoint).Address + ":" + ((IPEndPoint)receiveSocket.LocalEndpoint).Port);

		}

		protected override void CloseSendSocket() {
			base.CloseReceiveSocket ();

			if (sendSocket != null) {
				sendSocket.Close ();
				sendSocket = null;
			}
			if(sendStream != null) {
				sendStream.Close ();
				sendStream = null;
			}
		}

		protected override void CloseReceiveSocket() {
			base.CloseReceiveSocket ();

			if (receiveSocket != null) {
				receiveSocket.Stop();
				receiveSocket = null;
			}
			if (receiveStream != null) { 
				receiveStream.Close();
				receiveStream = null;
			}
		}

		// -- Sending Functionality

		// Send packet
		public override void SendPacket(byte[] a_buffer)
		{
			base.OnSendPacket (a_buffer);

			sendStream.Write (a_buffer, 0, a_buffer.Length);

			OnSendPacket(a_buffer);
		}

		// -- Getters and Setters
		public override string ReceiveAddress { get { return ((IPEndPoint)receiveSocket.LocalEndpoint).Address.ToString(); }}
		public override int ReceivePort { get { return ((IPEndPoint)receiveSocket.LocalEndpoint).Port; }}

		public override string SendAddress { get { return ((IPEndPoint)sendSocket.Client.LocalEndPoint).Address.ToString(); }}
		public override int SendPort { get { return ((IPEndPoint)sendSocket.Client.LocalEndPoint).Port; }}

		// -- Listening Functionality

		// Handle listening
		protected override void ListenCallback()
		{
			base.ListenCallback ();
			Socket sc = receiveSocket.AcceptSocket ();
			receiveStream = new NetworkStream (sc);

            byte[] bytes = new byte[1024];

            while (isListening)
			{
               // using(NetworkStream stream = receiveStream)
				//try
				{
					//TcpClient cc = receiveSocket.AcceptTcpClient ();

                    int length;
					while((length = receiveStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        byte[] incomingData = new byte[length];
                        Array.Copy(bytes, 0, incomingData, 0, length);

                        //string msg = Encoding.ASCII.GetString(incomingData);
                        OnReceivePacket(incomingData, null);

                    }
                    //Debug.Log (Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1));

                    /*NetworkStream stream = new NetworkStream(receiveSocket);
					byte[] buffer = new byte[128];
					int responseLength = stream.Read(buffer, 0, 128);
					Debug.Log(buffer);*/
                    //receiveSock
                    //Thread.Sleep(10);
                }
			}
		}
	}
}
