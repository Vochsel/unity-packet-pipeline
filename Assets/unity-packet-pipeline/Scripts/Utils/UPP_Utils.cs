using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System;

namespace UnityPacketPipeline
{
    /**
     *  Desc: Collection of utilities to handle packets and messages in the UPP system
     *
     */
    public struct UPP_Utils
    {
        public static string Vector3ToString(Vector3 a_vector)
        {
            return a_vector.ToString("G5");
        }

        public static Vector3 StringToVector3(string a_vector)
        {
			//Debug.Log (a_vector);

			a_vector = a_vector.Replace ("(", "");
			a_vector = a_vector.Replace (")", "");

			//Debug.Log (a_vector);

            // split the items
            string[] sArray = a_vector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}