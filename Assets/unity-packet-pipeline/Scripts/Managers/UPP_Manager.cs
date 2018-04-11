using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using System.Net;
using System.Net.Sockets;

using System.Text;
using System;


namespace UnityPacketPipeline
{
	/**
	* Name: UPP_PacketProtocl
	*
	* Auth: Ben Skinner
	* Date: 28/03/18
	* Desc: Enumeration to describe the two currently supported forms of network communication: UDP and TCP
	*/
	[System.Serializable]
	public enum UPP_PacketProtocol
	{
		UDP,
		TCP
	}

    [System.Serializable]
    public enum UPP_ManagerStatus
    {
        CONNECTED,
        CONNECTING,
        CLOSED,
        CLOSING
    }


	/**
	* Name: OnSetupEvent
	*
	* Auth: Ben Skinner
	* Date: 28/03/18
	* Desc: Wrapper for manager setup completion
	*/
	[System.Serializable]
	public class OnManagerSetupEvent : UnityEvent<UPP_Manager, UPP_Base_TwoWaySocket> { }

    /**
    *  Name: UPP_Manager
    *
    *  Auth: Ben Skinner
    *  Date: 28/03/18
    *  Desc: Unity component to manage tracked UPP_Components. Handles send and receive functionality for trackedComponents.
    */

    public class UPP_Manager : MonoBehaviour
    {

        // -- Public Static Variables

        // Singleton instance
        public static UPP_Manager MainUPPManager;

        // -- Public Manager Settings

		// Packet protocol to use
		public UPP_PacketProtocol Protocol;

        // Remote IP of upp connection
        public string RemoteIP = "127.0.0.1";

        // Port to both send packets to and listen for packets on
        public int Port = 4000;

		// If given tag will only search tagged objects for UPP_Components 
		public string AssociatedTag = "";

		// Event called on manager successfully setup
		public OnManagerSetupEvent OnSetup;
		
        // -- Private Member Variables

        // UDP Socket used for communication
        UPP_Base_TwoWaySocket twoWaySocket;

        // All components in the scene to track
        List<UPP_Component> trackedComponents;

        public List<UPP_Component> ConnectedComponents { get { return trackedComponents; } }

        private UPP_ManagerStatus _managerStatus = UPP_ManagerStatus.CLOSED;

        public UPP_ManagerStatus ManagerStatus { get { return _managerStatus; } set { _managerStatus = value; } }

        public UPP_Base_TwoWaySocket GetTwoWaySocket { get { return (twoWaySocket == null) ? null : twoWaySocket; } }

        public string SendSocket { get { return (twoWaySocket != null && twoWaySocket.CanSend) ? (twoWaySocket.SendAddress + " : " + twoWaySocket.SendPort.ToString()) : "Disconnected!"; } }
        public string ReceiveSocket { get { return (twoWaySocket != null && twoWaySocket.CanReceive) ? (twoWaySocket.ReceiveAddress + " : " + twoWaySocket.ReceivePort.ToString()): "Disconnected!"; } }

        // -- Unity Events

        // Link and register manager functionality
        void Start()
        {
            // Store singleton instance
            MainUPPManager = this;
                        
            if (twoWaySocket == null)
            {
                SetupManager(RemoteIP, Port);
                RefreshComponents();
            }
        }

		// Clean up network connection
		void OnApplicationQuit()
		{
			CloseManager ();
		}

        void OnEnable()
        {
            if (twoWaySocket == null)
            {
                SetupManager(RemoteIP, Port);
                RefreshComponents();
            }
        }

        void OnDisable()
        {
            CloseManager();
        }

		// -- Lifecycle functionality

		public void SetupManager(string a_ip, int a_port)
		{
			try {
				// Setup network connection
				switch (Protocol) {

				case UPP_PacketProtocol.TCP:
					twoWaySocket = new UPP_TCP_TwoWaySocket (a_ip, a_port);
					break;

				default: case UPP_PacketProtocol.UDP:
					twoWaySocket = new UPP_UDP_TwoWaySocket (a_ip, a_port);
					break;
				}

				// Register callback on packet received
				twoWaySocket.ReceivePacketHook = ReceiveMessage;
			} catch (SocketException e) {
				Debug.LogFormat("Failed to setup UPP Manager {0}: {1}", this.name, e);
			}
			OnSetup.Invoke (this, twoWaySocket);
		}

		public void CloseManager()
		{
            _managerStatus = UPP_ManagerStatus.CLOSING;

            if (twoWaySocket != null)
				twoWaySocket.Close();

			twoWaySocket = null;

            _managerStatus = UPP_ManagerStatus.CLOSED;
        }

		public void RefreshManager() {
			CloseManager ();
			SetupManager (RemoteIP, Port);
		}

		public void RefreshSendSockets()
		{
			twoWaySocket.RefreshSendSocket (RemoteIP, Port);
		}


		public void RefreshReceiveSockets()
		{
			twoWaySocket.RefreshReceiveSocket (RemoteIP, Port);
		}


		public void RefreshComponents()
		{
            _managerStatus = UPP_ManagerStatus.CONNECTING;

            // Clear old component list
            if (trackedComponents != null)
				trackedComponents.Clear ();
			
			// Populate tracked components
			if (AssociatedTag.Length > 0) {
				trackedComponents = new List<UPP_Component> ();
				GameObject[] gos = GameObject.FindGameObjectsWithTag (AssociatedTag);
				foreach (GameObject go in gos) {
                    if (!go.activeSelf)
                        continue;
					//If has UPP_Component, add
					if (go.GetComponent<UPP_Component> ()) {
						//Account for multiple on the one object
						foreach (UPP_Component uppc in go.GetComponents<UPP_Component>()) {
                            if (!uppc.isActiveAndEnabled)
                                continue;

							trackedComponents.Add (uppc);
						}
					}
				}
			} else {
				// Else find all scene UPP_Components
				trackedComponents = new List<UPP_Component>(FindObjectsOfType<UPP_Component>());
			}

			Debug.LogFormat("[{0}] Populated with {1} components", this.name + "-" + this.Protocol.ToString(), trackedComponents.Count);

			// Connect all components to this
			foreach (UPP_Component uppc in trackedComponents) {
				uppc.Connect (this);
			}

            _managerStatus = UPP_ManagerStatus.CONNECTED;
        }

		public void ChangeRemoteIP (string a_ip)
		{
			RemoteIP = a_ip;
			RefreshManager();
		}

        // -- Public Utility Functions

        // Send packet with prepended component ID in buffer
        public void SendComponent(UPP_Component a_component, byte[] a_message)
        {
            // Pre checks
            if (a_component == null)
                return;

            if (twoWaySocket == null)
            {
                CloseManager();
                return;
            }

            if (a_message.Length <= 0)
                return;

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
            BroadcastMessage(a_buffer);
        }

        // Send buffer to listening components
        // TODO: Convert trackedComponents to Map of arrays with ID as key
        void BroadcastMessage(byte[] a_buffer)
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