using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


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
				receiveSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 3001);
				receiveSocket.Start();
				StartListening();

				sendSocket = new TcpClient ();
				sendSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3001));
				sendStream = sendSocket.GetStream();
			} catch(SocketException e) {
				Debug.Log ("Could not create sockets");
				Debug.Log (e);
			}
			// Connect sending socket
			//sendSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3001));

			// Debug Logs
			//Debug.Log("Receiving Socket: " + ((IPEndPoint)receiveSocket.Client.LocalEndPoint).Address + ":" + ((IPEndPoint)receiveSocket.Client.LocalEndPoint).Port);
			//Debug.Log("Send Socket: " + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Address + ":" + ((IPEndPoint)sendSocket.Client.LocalEndPoint).Port);

			// Begin listening thread

		}

		// Correctly closes sockets and stops thread
		public override void Close()
		{
			base.Close ();

			// Stop listening thread
			StopListening();

			// Close sockets
			if (sendSocket != null) {
				sendSocket.Close ();
				sendStream.Close ();
			}
			if (receiveSocket != null) receiveStream.Close();
		}

		// -- Sending Functionality

		// Send packet
		public override void SendPacket(byte[] a_buffer)
		{
			base.OnSendPacket (a_buffer);

			sendStream.Write (a_buffer, 0, a_buffer.Length);

			OnSendPacket(a_buffer);
		}

		// -- Listening Functionality

		// Handle listening
		protected override void ListenCallback()
		{
			base.ListenCallback ();
			Socket sc = receiveSocket.AcceptSocket ();
			receiveStream = new NetworkStream (sc);
			while (isListening)
			{
				//try
				{
					//TcpClient cc = receiveSocket.AcceptTcpClient ();



					byte[] buffer = new byte[128];
					int responseLength = receiveStream.Read(buffer, 0, 128);
					receiveStream.Read (buffer, 0, 128);
					//Debug.Log (Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1));

					/*NetworkStream stream = new NetworkStream(receiveSocket);
					byte[] buffer = new byte[128];
					int responseLength = stream.Read(buffer, 0, 128);
					Debug.Log(buffer);*/

					OnReceivePacket(buffer, null);
					//Thread.Sleep(10);
				}
			}
		}
	}
}
