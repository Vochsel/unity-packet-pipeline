using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UPP_TransformComponent : UPP_Component {

    Vector3 receivedPos;
    Vector3 receivedRot;
    
    protected override void Update()
    {
        base.Update();

        if (clientType == UPP_Types.CLIENT_TYPE.Receive)
        {
            transform.position = receivedPos;
            transform.rotation = Quaternion.Euler(receivedRot);
        }
    }


    protected override void SendData()
    {
        base.SendData();

        string msg = transform.position.ToString("G5") + ":" + transform.rotation.eulerAngles.ToString("G5");

        UPP_Manager.MainUPPManager.SendComponent(this, msg);
    }

    public override void ReceiveData(string a_data)
    {
        base.ReceiveData(a_data);

        string[] splitComps = a_data.Split(':');

        receivedPos = UPP_Utils.StringToVector3(splitComps[0]);
        receivedRot = UPP_Utils.StringToVector3(splitComps[1]);
    }
}
