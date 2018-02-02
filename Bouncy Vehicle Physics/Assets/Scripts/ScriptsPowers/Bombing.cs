using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bombing : NetworkBehaviour {

    private Rigidbody bombBody;
    public float delay = 2f;
    public float radiu = 50000f;
    public float force = 10000000000000000000000000f;

    float countdown;
    bool hasExploded = false;
    public GameObject explosionEffect;

	// Use this for initialization
	void Start () {
        countdown = delay;
        bombBody = transform.GetComponent<Rigidbody>();
        bombBody.AddForce(transform.forward * 7500f);
        bombBody.AddForce(transform.up * 500f);
    }

    void Explode()
    {
        
        CmdExplode(force, transform.position, radiu, 3.0F);
    }

    [Command]
    void CmdExplode(float power, Vector3 explosionPos, float radius, float upwardsMod)
    {
        GameObject exp = Instantiate(explosionEffect, transform.position, transform.rotation);
        NetworkServer.Spawn(exp);
        RpcExplode(power, explosionPos, radius, upwardsMod);
    }

    [ClientRpc]
    void RpcExplode(float power, Vector3 explosionPos, float radius, float upwardsMod)
    {
        
        Destroy(gameObject);

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Amarelo" || collision.gameObject.name == "Azul" || collision.gameObject.name == "Vermelho" || collision.gameObject.name == "Verde")
            Explode();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shield") || other.CompareTag("Field"))
            Explode();
    }
}
