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

        private string latestMessage = "";

        // -- Public Variables

        // Internal component id
        public int ID = 0;

        // Type of client: Send/Receive
        public CLIENT_TYPE clientType;

        // -- Unity Events

        // Virtual protected Unity Start event
        protected virtual void Start() { }

        // Virtual protected Unity Update event
        protected virtual void Update()
        {
            if (clientType == CLIENT_TYPE.Send)
                SendData(StringifyData());
            else if (clientType == CLIENT_TYPE.Receive && latestMessage.Length > 0)
                ParseData(latestMessage);
        }

        // -- Internal Functions

        // Send Data Function
        private void SendData(string a_data) { UPP_Manager.MainUPPManager.SendComponent(this, a_data); }

        // Called when data recevied on this component
        public void ReceiveData(string a_data) { latestMessage = a_data; }

        // -- Virtual Functions

        // Converts data to string to be sent
        public virtual string StringifyData() { return ""; }

        // Called on main thread and parses latest message
        public virtual void ParseData(string a_data) { }
    }

}