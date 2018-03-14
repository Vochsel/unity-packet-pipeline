using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace UnityPacketPipeline
{
	[System.Serializable]
	public class CallbackEvent : UnityEvent<string> {}

	[System.Serializable]
	public struct UPP_Callback {
		public string CallbackName;
		public CallbackEvent Callback;
	}

	/**
     *  Name: UPP_TransformComponent
     *  
     *  Auth: Ben Skiner
     *  Date: 8/03/18
     *  Desc: Extends UPP Component and handles sending and receiving of position and rotation.
     */
	public class UPP_CallbackComponent : UPP_Component
	{
		public UPP_Callback[] Callbacks;

		public void InvokeCallback(string a_callbackID, string a_callbackArg = "")
		{
			SendData (a_callbackID + ":" + a_callbackArg);

			foreach (UPP_Callback callback in Callbacks) {
				if (callback.CallbackName == a_callbackID) {
					callback.Callback.Invoke(a_callbackArg);
				}
			}
		}

		// -- Overriden internal functionality

		public override void ParseData(string a_data)
		{
			base.ParseData(a_data);

			string[] splitComps = a_data.Split(':');
			string parsedCallbackID = splitComps[0];
			string parsedCallbackArg = splitComps[1];

			foreach (UPP_Callback callback in Callbacks) {
				if (callback.CallbackName == parsedCallbackID) {
					callback.Callback.Invoke(parsedCallbackArg);
				}
			}
		}

		protected override void Update ()
		{
			base.Update ();
			if (clientType == CLIENT_TYPE.Receive && latestMessage.Length > 0) {
				ParseData (latestMessage);
				latestMessage = "";
			}
		}
	}
}