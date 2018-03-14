using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPacketPipeline
{

    /**
     *  Name: UPP_CameraComponent
     *
     *  Auth: Ben Skiner
     *  Date: 8/03/18
     *  Desc: Unity component to send camera data
     */
    [RequireComponent(typeof(Camera))]
    public class UPP_CameraComponent : UPP_Component
    {
        protected Camera localCamera;

        // -- Unity Events

        protected override void Start()
        {
            base.Start();

            // Link camera component
            localCamera = GetComponent<Camera>();
        }

		protected override void Update ()
		{
			base.Update ();

			if (clientType == CLIENT_TYPE.Send)
				SendData(StringifyData());
			else if (clientType == CLIENT_TYPE.Receive && latestMessage.Length > 0)
				ParseData(latestMessage);
		}

        // -- Overriden functions

        public override string StringifyData()
        {
            base.StringifyData();

            return UPP_Utils.Vector3ToString(transform.position) + ":" + UPP_Utils.Vector3ToString(transform.rotation.eulerAngles) + ":" + localCamera.fieldOfView;
        }

        public override void ParseData(string a_data)
        {
            base.ParseData(a_data);

            string[] splitComps = a_data.Split(':');

            transform.position = UPP_Utils.StringToVector3(splitComps[0]);
            transform.rotation = Quaternion.Euler(UPP_Utils.StringToVector3(splitComps[1]));
            localCamera.fieldOfView = float.Parse(splitComps[2]);
        }
    }
}