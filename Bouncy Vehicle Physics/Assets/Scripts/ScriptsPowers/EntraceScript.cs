using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EntraceScript : NetworkBehaviour {

    [SyncVar(hook = "setCar")] public string name;
    [SyncVar(hook = "setExit")] public Vector3 exit;
    private bool tele;
    private GameObject target;
    private float time;
    // Use this for initialization
    void Start () {
        tele = false;
        time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(tele)
        {
            target.transform.position = exit;
            foreach(GameObject i in GameObject.FindGameObjectsWithTag("Exit")) {
                Debug.Log("entrei --" + i.name);
                if(i.transform.position.Equals(exit))
                {
                    i.GetComponent<TeletransportScript>().destroyEl(true);
                }
            }
            CmdDestroyObject(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player1") && Time.time - time >= 1) {
            tele = true;
            target = GameObject.Find(other.name);
        } else if (other.CompareTag("Field"))
        {
            foreach (GameObject i in GameObject.FindGameObjectsWithTag("Exit"))
            {
                Debug.Log("entrei --" + i.name);
                if (i.transform.position.Equals(exit))
                {
                    i.GetComponent<TeletransportScript>().destroyEl(true);
                }
            }
            CmdDestroyObject(gameObject);
        }
    }
    public void setCar(string name)
    {
        this.name = name;
    }
    public void setExit(Vector3 pos)
    {
        this.exit = pos;
    }
    [Command]
    void CmdDestroyObject(GameObject ob)
    {
        NetworkServer.Destroy(ob);
    }
}
