using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CarCheckpoint : NetworkBehaviour {

    public List<GameObject> checkPointArray = new List<GameObject>(); //Checkpoint GameObjects stored as an array
    [SyncVar (hook = "setCheck")] public int currentCheckpoint = 0; //Current checkpoint
    [SyncVar(hook = "setLap")] public int currentLap = 0; //Current lap
    
    void Start()
    {
        setCheckArray();
        if (GameObject.Find("car1"))
        {
            gameObject.name = "car2";
        }
        else if (GameObject.Find("car2")){
            gameObject.name = "car3";

        }else if (GameObject.Find("car3"))
        {
            gameObject.name = "car4";

        }else
        {
            gameObject.name = "car1";
        }
       

    }

    public void setCheckArray()
    {
        //Debug.Log(GameObject.Find("CheckPoints").transform.childCount);
        foreach (Transform obj in GameObject.Find("CheckPoints").transform)
        {
            if (obj.tag == "Check")
                checkPointArray.Add(obj.gameObject);
        }
    }

    public int getCurrentCheck()
    {
        return currentCheckpoint;
    }
    public int getCurrentLap()
    {
        return currentLap;
    }


    public void setCheck(int val)
    {
        currentCheckpoint = val;
        if (!isServer)
            CmdsetCheck(val);
    }

    [Command]
    public void CmdsetCheck(int val)
    {
        currentCheckpoint = val;
    }


    public void setLap(int lap)
    {
        currentLap = lap;
        if(!isServer)
            CmdsetLap(lap);
    }
    [Command]
    public void CmdsetLap(int lap)
    {
        currentLap = lap;
    }
}
