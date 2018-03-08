using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UPP_Component : MonoBehaviour {

    public int ID = 0;

    public UPP_Types.CLIENT_TYPE clientType;

    protected virtual void Update()
    {
        if (clientType == UPP_Types.CLIENT_TYPE.Send)
            SendData();
    }

    protected virtual void SendData() { }

    public virtual void ReceiveData(string a_data) { }
}
