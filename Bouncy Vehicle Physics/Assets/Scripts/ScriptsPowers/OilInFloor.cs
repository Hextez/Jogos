using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilInFloor : MonoBehaviour {

    private float effectForce = 1000f;
    private float normalForce = 25000f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
            collision.gameObject.GetComponent<HoverCarControl>().forwardAcceleration = effectForce;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
            collision.gameObject.GetComponent<HoverCarControl>().forwardAcceleration = normalForce;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Field"))
        {
            Destroy(gameObject);
        }
    }

}
