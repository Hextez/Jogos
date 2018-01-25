using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mining : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            Destroy(gameObject);

        }

        if (other.name == "Shield")
        {
            Destroy(gameObject);
        }

    }
}
