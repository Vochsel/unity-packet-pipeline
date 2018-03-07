using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;

public class TransformHandler : MonoBehaviour {

    UDPSocket udpClient;

    public NPP_Types.CLIENTS clientType;

    IPEndPoint remote;
    public string remoteAddress = "127.0.0.1";
    public int remotePort;

    Vector3 receivedPosition;
    Vector3 receivedEulerRot;

    float receivedFov;
    bool receivedRecording;

    public Camera linkedCam;

    // Use this for initialization
    void Start () {

        bool shouldListen = (clientType == NPP_Types.CLIENTS.SendReceive || clientType == NPP_Types.CLIENTS.Receive) ? true : false;

        remote = new IPEndPoint(IPAddress.Parse(remoteAddress), remotePort);

        udpClient = new UDPSocket(shouldListen);
        udpClient.ReceivePacketHook += ReceiveMessage;

        //SendMessage(NPP_Types.MESSAGES.Connection, (byte)clientType);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }

    // Update is called once per frame
    void Update () {
        switch (clientType) {
            case NPP_Types.CLIENTS.Send:
                SendTransform(transform);

                break;
            case NPP_Types.CLIENTS.Receive:
                transform.position = receivedPosition + Vector3.right * 2;
                transform.rotation = Quaternion.Euler(receivedEulerRot);

                linkedCam.fieldOfView = receivedFov;

                break;

            case NPP_Types.CLIENTS.SendReceive:

                SendTransform(transform);

                transform.position = receivedPosition;
                transform.rotation = Quaternion.Euler(receivedEulerRot);
                break;
        }
    }

    protected void SendMessage(NPP_Types.MESSAGES a_type, byte[] a_buffer)
    {
        // Create message buffer
        byte[] buffer = new byte[1 + a_buffer.Length];
       // byte[] buffer = a_buffer;

        // Prepend message type
        buffer[0] = (byte)a_type;

        // Append bufer
        System.Buffer.BlockCopy(a_buffer, 0, buffer, 1, a_buffer.Length);

        // Send packet
        udpClient.SendPacket(buffer, new IPEndPoint(IPAddress.Parse(remoteAddress), 3000));
    }

    protected void SendMessage(NPP_Types.MESSAGES a_type, string a_buffer)
    {
        SendMessage(a_type, Encoding.UTF8.GetBytes(a_buffer));
    }

    protected void SendMessage(NPP_Types.MESSAGES a_type, byte a_buffer)
    {
        SendMessage(a_type, new byte[] { a_buffer });
    }

    protected void SendTransform(Transform a_transform)
    {
        SendMessage(NPP_Types.MESSAGES.Data, a_transform.position.ToString("G5") + ':' + a_transform.rotation.eulerAngles.ToString("G5"));
    }

    protected void SendCamera(Camera a_camera, bool a_isRecording)
    {
        int recordingVal = (a_isRecording) ? 1 : 0;
        SendMessage(NPP_Types.MESSAGES.Data, a_camera.transform.position.ToString("G5") + ':' + a_camera.transform.rotation.eulerAngles.ToString("G5") + ':' + a_camera.fieldOfView + ':' + recordingVal);
    }

    void ReceiveMessage(byte[] a_buffer, IPEndPoint a_remote)
    {
       NPP_Types.MESSAGES messageType = (NPP_Types.MESSAGES )a_buffer[0];

        string messageData = Encoding.UTF8.GetString(a_buffer, 1, a_buffer.Length - 1);
        //string messageData = Encoding.UTF8.GetString(a_buffer);

        switch (messageType)
        {
            case NPP_Types.MESSAGES.Data:
                ReceiveTransformAsString(messageData);
                break;
        }
    }

    void ReceiveTransformAsString(string a_transform)
    {
        string[] transformComponents = a_transform.Split(':');

        Vector3 position = NPP_Utils.StringToVector3(transformComponents[0]);
        Vector3 rotation = NPP_Utils.StringToVector3(transformComponents[1]);

        if (transformComponents.Length > 2)
        {

            float fov = float.Parse(transformComponents[2]);
            bool isRecording = bool.Parse(transformComponents[3]);

            receivedPosition = position;
            receivedEulerRot = rotation;

            receivedFov = fov;
            receivedRecording = isRecording;
        }
    }


}
