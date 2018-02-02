using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CountDown : NetworkBehaviour {

    public Text count;
    private float startTime;
    [SyncVar (hook="setTemp")] public string texto;

    // Use this for initialization
    void Start () {
        startTime = 5;
        count = GetComponent<Text>();
    }


    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Player1").Length > 0)
        {
            Debug.Log("CountDOw");
            float t = startTime - Time.time;
            if (t > -1)
            {
                string seconds = (t % 60).ToString("f2");
                if (isServer)
                {
                    setTemp(seconds);
                }
                count.text = texto;
                // Debug.Log(t);
                if (t < 0)
                {
                    count.text = "GO!!!";
                    if (isServer) { 
                        foreach(GameObject d in GameObject.FindGameObjectsWithTag("Player1"))
                        {
                            d.GetComponent<HoverCarControl>().setMove(true);
                        }
                    }
                }
            }
            if (t < -1)
            {
                Destroy(count);

            }
        }
        

        
    }

    public void setTemp(string s)
    {
        texto = s;
       RpcsendClient(texto);
    }

    [ClientRpc]
    public void RpcsendClient(string s)
    {
        texto = s;
    }
}
