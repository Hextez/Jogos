using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPunch : MonoBehaviour {

    public float force = 40f;
    private Rigidbody glove;
    public string position;

    void setPosition(string pos)
    {
        position = pos;
    }

	// Use this for initialization
	void Start () {
        glove = transform.GetComponent<Rigidbody>();
        new WaitForSeconds(2);
        if (position == "back")
        {
            transform.Rotate(90,180, 0);
        }
        else if (position == "front")
        {
            transform.Rotate(90, 0, 0);
        }
        else if (position == "right")
        {
            transform.Rotate(90, 90, 0);
        }
        else
        {
            transform.Rotate(90, -90, 0);
        }


        glove.velocity = transform.up * force;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
