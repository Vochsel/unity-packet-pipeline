using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UnityPacketPipeline
{
	// Delegate on packet received
	public delegate void ReceivePacketDelegate(byte[] a_buffer, IPEndPoint a_remote);


	public abstract class UPP_Base_TwoWaySocket
	{
		// -- Member variables

		// Instance of delegate fired when packet received
		public ReceivePacketDelegate ReceivePacketHook;

		// Bool which controls if the connection is listening
		protected bool isListening = false;

		protected Thread receiveThread;

		// -- Open and Close

		public virtual void Open(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000) {}

		public virtual void Close() {}

		// -- Send Functionality
		public virtual void SendPacket(byte[] a_buffer) {}

		// -- Listening Functionality

		// Handle thread startup
		protected virtual void StartListening() {
			// Create thread
			receiveThread = new Thread(new ThreadStart(ListenCallback));
			receiveThread.IsBackground = true;

			// Start thread
			receiveThread.Start();
			isListening = true;
		
			Debug.Log("Started listening");

		}

		// Handle thread closing
		protected virtual void StopListening() {
			if (receiveThread != null && receiveThread.IsAlive)
			{
				isListening = false;
				receiveThread.Abort();
			}
		}

		// Handle listening
		protected virtual void ListenCallback() {
			OnListening();
		}

		// -- Getters and Setters

		public abstract string ReceiveAddress { get; }
		public abstract int ReceivePort { get; }

		public abstract string SendAddress { get; }
		public abstract int SendPort { get; }

		// -- Callbacks

		// Called when connection starts listening
		public virtual void OnListening() {}

		// Called when connection sends packet
		public virtual void OnSendPacket(byte[] a_buffer) { }

		// Called when connection receives packet
		public virtual void OnReceivePacket(byte[] a_buffer, IPEndPoint a_remote) { ReceivePacketHook(a_buffer, a_remote); }
	}
}
