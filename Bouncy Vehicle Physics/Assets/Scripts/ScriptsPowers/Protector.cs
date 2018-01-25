using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protector : MonoBehaviour {
    
    public GameObject SpawnPosIn;
    Transform shield;
    private float endTime;

	// Use this for initialization
	void Start () {
        shield = gameObject.transform;
        endTime = Time.time + 3;
        Color c = gameObject.GetComponent<Renderer>().material.color;
        c.a = 0.2f;
        gameObject.GetComponent<Renderer>().material.color = c;

    }
	
	// Update is called once per frame
	void Update () {
        shield.position = SpawnPosIn.transform.position;

        if (endTime - Time.time < 0)
            Destroy(gameObject);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "missile" || other.name == "Mina")
        {
            Destroy(gameObject);

        }

    }

    public void setPosPosition(GameObject spawn)
    {
        SpawnPosIn = spawn;
    }
}
