using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBomb : MonoBehaviour {

    public float throwForce = 40f;
    public GameObject bomb;
    
    
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("FireP1"))
        {
            ThrowShit ();
        }
	}

    void ThrowShit()
    {
        GameObject grenade = Instantiate(bomb, transform.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce);
        rb.AddForce(transform.up * throwForce);
    }
}
