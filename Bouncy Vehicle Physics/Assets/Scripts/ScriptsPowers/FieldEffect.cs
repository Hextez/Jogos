using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class FieldEffect : NetworkBehaviour
{
    [SyncVar(hook = "setCar")] public string name;
    public float rotationSpeed;
    public float endTime;
    public GameObject[] powersAffected;
    // Use this for initialization
    void Start()
    {
        endTime = Time.time + 3;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed, 0);
        transform.position = GameObject.Find(name).transform.Find("SpawnPointMiddle").transform.position;
        if (endTime < Time.time)
        {
            if (isServer)
                gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(gameObject.GetComponent<NetworkIdentity>().clientAuthorityOwner);

            GameObject.Find(name).GetComponent<HoverCarControl>().setProtected(false);
            CmdDestroyObject(gameObject);
        }
    }
    public void setCar(string name)
    {
        this.name = name;
    }

    [Command]
    void CmdDestroyObject(GameObject ob)
    {
        NetworkServer.Destroy(ob);
    }
}
