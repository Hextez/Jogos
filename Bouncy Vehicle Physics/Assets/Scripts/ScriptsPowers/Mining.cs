using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mining : NetworkBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            if (isServer)
                gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(gameObject.GetComponent<NetworkIdentity>().clientAuthorityOwner);
            NetworkServer.Destroy(gameObject);


        }

        if (other.CompareTag("Shield"))
        { 
            if (isServer)
                gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(gameObject.GetComponent<NetworkIdentity>().clientAuthorityOwner);
            NetworkServer.Destroy(gameObject);

        }

    }
}
