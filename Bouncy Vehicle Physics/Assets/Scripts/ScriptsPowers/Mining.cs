using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mining : NetworkBehaviour {

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Shield") || other.CompareTag("Field"))
        {
            NetworkServer.Destroy(gameObject);

        }

        if (other.CompareTag("Player1"))
        {
           NetworkServer.Destroy(gameObject);


        }

        

    }
}
