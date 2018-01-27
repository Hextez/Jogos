﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Protector : NetworkBehaviour {

    [SyncVar(hook = "setPosPosition")] public string name;
    public Transform car;
    public GameObject Car;
    Transform shield;
    private float endTime;

	// Use this for initialization
	void Start () {
        

        //car = gameObject.transform.parent;

        //shield = gameObject.transform;
        endTime = Time.time + 3;
        Color c = gameObject.GetComponent<Renderer>().material.color;
        c.a = 0.2f;
        gameObject.GetComponent<Renderer>().material.color = c;

    }
	
	// Update is called once per frame
	void Update () {
        
        transform.position = GameObject.Find(name).transform.Find("SpawnPointMiddle").transform.position;
        if (endTime - Time.time < 0)
            Destroy(gameObject);
        

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Missile(Clone)" || other.name == "Mina")
        {
            Destroy(gameObject);

        }

    }

    public void setPosPosition(string spawn)
    {
        name = spawn;
    }
}
