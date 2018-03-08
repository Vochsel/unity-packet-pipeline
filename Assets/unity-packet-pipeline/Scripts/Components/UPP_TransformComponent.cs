using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityPacketPipeline
{
    /**
     *  Name: UPP_TransformComponent
     *  
     *  Auth: Ben Skiner
     *  Date: 8/03/18
     *  Desc: Extends UPP Component and handles sending and receiving of position and rotation.
     */
    public class UPP_TransformComponent : UPP_Component
    {

        // -- Overriden internal functionality

        public override string StringifyData()
        {
            base.StringifyData();

            return UPP_Utils.Vector3ToString(transform.position) + ":" + UPP_Utils.Vector3ToString(transform.rotation.eulerAngles);
        }

        public override void ParseData(string a_data)
        {
            base.ParseData(a_data);

            string[] splitComps = a_data.Split(':');

            transform.position = UPP_Utils.StringToVector3(splitComps[0]);
            transform.rotation = Quaternion.Euler(UPP_Utils.StringToVector3(splitComps[1]));
        }
    }
}