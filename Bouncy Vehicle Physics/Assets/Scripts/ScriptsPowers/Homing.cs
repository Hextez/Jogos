using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : MonoBehaviour {

    private float missileVelocity = 100;
    private float turn = 20;
    public Rigidbody homingMissile;
    public float fuseDelay;
    public GameObject missileMod;
    
    public Transform target;

    void Start()
    {
        homingMissile = transform.GetComponent<Rigidbody>();
        new WaitForSeconds(2);
        Fire();

    }

    public void setTransform(Transform s)
    {
        target = s;
    }

    void FixedUpdate()
    {
        if (target == null || homingMissile == null)
            return;

        homingMissile.velocity = transform.forward * missileVelocity;

        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        homingMissile.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, turn));

    }

    void Fire()
    {
        new WaitForSeconds(fuseDelay);

        float distance = Mathf.Infinity;
 
    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Target"))
        {
            float diff = (go.transform.position - transform.position).sqrMagnitude;

            if (diff < distance)
            {
                distance = diff;
                target = go.transform;
            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player1"))
        {
            Destroy(gameObject);

        }

        if(other.name == "Shield")
        {
            Destroy(gameObject);

        }

    }

}
