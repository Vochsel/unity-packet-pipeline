using UnityEngine;
using System.Collections;

public class UPP_ButtonTest : MonoBehaviour
{
	void OnGUI()
	{
		if(GUI.Button(new Rect(10, 10, 200, 10), "Test")) {
			GetComponent<UnityPacketPipeline.UPP_CallbackComponent> ().InvokeCallback ("Test");
		}
	}

	public void Test(string arg)
	{
		Debug.Log ("Test Worked!");
	}
}

