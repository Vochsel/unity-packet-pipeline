using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            // Remove the parentheses
            if (a_vector.StartsWith("(") && a_vector.EndsWith(")"))
            {
                a_vector = a_vector.Substring(1, a_vector.Length - 2);
            }

            // split the items
            string[] sArray = a_vector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }
    }
}