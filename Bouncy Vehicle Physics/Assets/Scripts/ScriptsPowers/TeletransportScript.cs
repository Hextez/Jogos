using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeletransportScript : NetworkBehaviour {


    public GameObject entrance;
    [SyncVar(hook = "setCar")] public string name;
    [SyncVar(hook = "setCreated")] public bool created;
    [SyncVar(hook = "destroyEl")] public bool destroy;
    [SyncVar(hook = "setCheckpoint")] public int checkpoint = -2;
    private float endTime;
    // Use this for initialization
    void Start () {
        endTime = Time.time + 1;
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time > endTime && !created)
        {
            setCreated(true);
            StartCoroutine(Entrance(1));
            //CmdCreateEntrace();
        }
        else if(checkpoint == -2)
        {
            setCheckpoint(GameObject.Find(name).GetComponent<CarCheckpoint>().getCurrentCheck());
        }
        if(destroy)
        {
            CmdDestroyObject(gameObject);
        }
	}

    private void setCreated(bool v)
    {
        created = v;
    }

    [Command]
    void CmdCreateEntrace()
    {
        Vector3 position = GameObject.Find(name).transform.Find("SpawnPointBack").transform.position;
        entrance = Instantiate(entrance, position, entrance.transform.rotation);
        entrance.GetComponent<EntraceScript>().setCar(name);
        entrance.GetComponent<EntraceScript>().setExit(transform.position);
        entrance.GetComponent<EntraceScript>().setCheckpoint(checkpoint);
        NetworkServer.Spawn(entrance);
    }

    [Command]
    void CmdDestroyObject(GameObject ob)
    {
        NetworkServer.Destroy(ob);
    }
    IEnumerator Entrance(float time)
    {
        var timer = 0.0f;
        while (timer <= time)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        if(created)
            CmdCreateEntrace();
    }
    public void setCar(string name)
    {
        this.name = name;
    }
    public void destroyEl(bool bol)
    {
        this.destroy = bol;
    }
    public void setCheckpoint(int i)
    {
        this.checkpoint = i;
    }
}
