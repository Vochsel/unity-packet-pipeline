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

        // -- Helper variables

        public virtual bool CanSend { get { return false; } }
        public virtual bool CanReceive { get { return false; } }

        public bool IsListening { get { if (receiveThread == null) return false; else return receiveThread.IsAlive; } }

        public int PacketsSent = 0;
        public int PacketsReceived = 0;

        // -- Open and Close

        public virtual void Open(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000) {}

		public virtual void Close() {}

		// -- Send Functionality
		public virtual void SendPacket(byte[] a_buffer) {
            if (!CanSend)
            {
                Debug.Log(string.Format("Cannot send packet! [{0}]", a_buffer));
                return;
            }

            PacketsSent++;
        }

		// -- Listening Functionality

		// Handle thread startup
		protected virtual void StartListening() {
			// Create thread
			try {
				receiveThread = new Thread(new ThreadStart(ListenCallback));
				receiveThread.IsBackground = true;

				// Start thread
				receiveThread.Start();
				isListening = true;
			
				Debug.Log("Started listening");
			} catch(ThreadAbortException e) {
				Debug.Log ("Listening aborted");
				Debug.Log (e);
			}

		}

		// Handle thread closing
		protected virtual void StopListening() {
			if (receiveThread != null /* && receiveThread.IsAlive*/)
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


		// -- Socket Lifecycle 

		protected virtual void OpenSendSocket(string a_remoteAddress, int a_listenPort) {
			// Close send socket if already open
			CloseSendSocket();
		}

		protected virtual bool OpenSendSocketAsync(string a_remoteAddress, int a_listenPort) {
			// Close send socket if already open
			CloseSendSocket();

			return true;
		}


		protected virtual void OpenReceiveSocket(string a_remoteAddress, int a_listenPort) {
			// Close receive socket if already open
			CloseReceiveSocket();
		}

		protected virtual bool OpenReceiveSocketAsync(string a_remoteAddress, int a_listenPort) {
			// Close receive socket if already open
			CloseReceiveSocket();

			return true;
		}


		protected virtual void CloseSendSocket() {
		}

		protected virtual void CloseReceiveSocket() {
		}


		public void RefreshSendSocket(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000) {
			CloseSendSocket ();
			OpenSendSocketAsync (a_remoteAddress, a_listenPort);
		}

		public void RefreshReceiveSocket(string a_remoteAddress = "127.0.0.1", int a_listenPort = 3000) {
			CloseReceiveSocket ();
			OpenReceiveSocket (a_remoteAddress, a_listenPort);
		}

		// -- Callbacks

		// Called when connection starts listening
		public virtual void OnListening() {}

		// Called when connection sends packet
		public virtual void OnSendPacket(byte[] a_buffer) {
        }

		// Called when connection receives packet
		public virtual void OnReceivePacket(byte[] a_buffer, IPEndPoint a_remote) {
            
            PacketsReceived++;

            ReceivePacketHook(a_buffer, a_remote);
        }
	}
}
