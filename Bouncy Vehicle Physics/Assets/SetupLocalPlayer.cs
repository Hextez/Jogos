using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour {


    [SyncVar] public string playername = "player";

    [SyncVar] public Color playerColor = Color.white;

	// Use this for initialization
	void Start () {
        //Debug.Log(playername);
        Transform childtoWork = null;

        foreach (Transform child in transform)
        {
            if (child.name == "Model")
            {
                childtoWork = child.transform.GetChild(0);
                Material mat = childtoWork.GetComponent<Renderer>().material;
                mat.color = playerColor;
            }
        }
       
	}
	
	
}
