using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombing : MonoBehaviour {

    private Rigidbody bombBody;
    public float delay = 4f;
    public float radiu = 5f;
    public float force = 700f;

    float countdown;
    bool hasExploded = false;
    public GameObject explosionEffect;

	// Use this for initialization
	void Start () {
        countdown = delay;
        bombBody = transform.GetComponent<Rigidbody>();
	}
    private void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radiu);
        foreach(Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radiu);
            }
        }

        Destroy(gameObject);
    }
}
