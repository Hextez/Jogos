using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Homing : NetworkBehaviour {

    private float missileVelocity = 100;
    private float turn = 20;
    public Rigidbody homingMissile;
    public float fuseDelay;
    public GameObject missileMod;
    
    public Transform target;
    [SyncVar(hook = "setTargetName")] public string Tname;


    void Start()
    {
        missileMod = gameObject;
        target = GameObject.Find(name).transform;
        homingMissile = transform.GetComponent<Rigidbody>();
        new WaitForSeconds(2);
        Fire();

    }

    public void setTargetName(string s)
    {
        Tname = s;
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

        GameObject go = GameObject.Find(Tname);

        float diff = (target.transform.position - transform.position).sqrMagnitude;

        if (diff < distance)
        {
                distance = diff;
                target = go.transform;
        }

        

    }

    private void OnTriggerEnter(Collider other)
    {

       if ((other.CompareTag("Shield") && other.GetComponent<Protector>().name == Tname) || 
            ( other.CompareTag("Field") && other.GetComponent<FieldEffect>().name == Tname)){

            NetworkServer.Destroy(gameObject);

        }

       if(other.CompareTag("Player1") && other.name == Tname)
        {
            NetworkServer.Destroy(gameObject);
        }



    }

}
