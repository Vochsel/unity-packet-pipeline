using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPacketPipeline
{

    /**
     *  Name: UPP_Component
     *
     *  Auth: Ben Skiner
     *  Date: 8/03/18
     *  Desc: Unity component to manage component sending and receiving of data. Designed to be extended to fit per case functionality.
     */
    public class UPP_Component : MonoBehaviour
    {
        // -- Internal Protected Variables

        protected string latestMessage = "";
	
		protected UPP_Manager connectedManager;

        // -- Public Variables

        // Internal component id
        public int ID = 0;

        // Type of client: Send/Receive
        public CLIENT_TYPE clientType;

        // -- Unity Events

        // Virtual protected Unity Start event
        protected virtual void Start() { }

        // Virtual protected Unity Update event
        protected virtual void Update() { }

        // -- Internal Functions

        // Send Data Function
		protected virtual void SendData(string a_data) { if(IsConnected()) connectedManager.SendComponent(this, a_data); }

        // Called when data recevied on this component
		public virtual void ReceiveData(string a_data) { latestMessage = a_data; }

		public void Connect(UPP_Manager a_manager) {
			connectedManager = a_manager;
		}

		public bool IsConnected() {
			return connectedManager != null;
		}

        // -- Virtual Functions

        // Converts data to string to be sent
        public virtual string StringifyData() { return ""; }

        // Called on main thread and parses latest message
        public virtual void ParseData(string a_data) { }
    }

}