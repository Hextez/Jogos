using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TimeControler : NetworkBehaviour {


    private float timeoffset;
    private float endTime;
    private float time = 3;
    private Text count;
    private bool started;
    // Use this for initialization
    void Start () {
        started = false;
        time = Time.time;
        if (isServer)
            RpcNotifyStart();
        count = GameObject.Find("CountDown").GetComponent<Text>();
    }


    [ClientRpc]
    public void RpcNotifyStart()
    {
        timeoffset = Time.time - time;
    }

    // Update is called once per frame
    void Update()
    {
        if(started)
        {
            time = endTime - Time.time;
            if (time > -1)
            {
                string seconds = (time % 60).ToString("f2");
                count.text = seconds;
                if (time < 0)
                {
                    count.text = "GO!!!";
                    if (isServer)
                    {
                        foreach (GameObject d in GameObject.FindGameObjectsWithTag("Player1"))
                        {
                            d.GetComponent<HoverCarControl>().setMove(true);
                        }
                    }
                }
            }
            if (time < -1)
            {
                Destroy(count);

            }
        }
        else
            if (GameObject.FindGameObjectsWithTag("Player1").Length >= 2)
        {

            started = true;
            syncedTime();
            time = 0;
        }
    }

    public void syncedTime()
    {
        if (isServer)
        {
            endTime = Time.time + 3;
        }
        else
        {
            endTime = Time.time + timeoffset + 3;
        }
    }

}
