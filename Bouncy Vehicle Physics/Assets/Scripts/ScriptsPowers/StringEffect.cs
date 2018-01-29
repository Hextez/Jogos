using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StringEffect : NetworkBehaviour{


    [SyncVar(hook = "setCarName")] public string name;
    [SyncVar(hook = "setSpawn")] public int spawnPos;

    void Start () {

         StartCoroutine(Stretch(0.3f, 125.0f, new Vector3(0,0,500 * -1550.0f)));

    } 

    void Update()
    {
        if(name != null)
        {
            if(spawnPos == 1)
            {
                transform.position = GameObject.Find(name).transform.Find("SpawnPoint").transform.position;
            } else if (spawnPos == 3)
            {
                transform.position = GameObject.Find(name).transform.Find("SpawnPointLeft").transform.position;
            } else if(spawnPos == 2)
            {
                transform.position = GameObject.Find(name).transform.Find("SpawnPointRight").transform.position;
            }
        }
          
    }


    IEnumerator Stretch(float time, float distance, Vector3 direction)
    {
        var timer = 0.0f;
        direction.Normalize();
       
        while (timer <= time)
        {
            transform.localScale += new Vector3(0, 0.015f, 0);
            yield return null;
            timer += Time.deltaTime;
        }
        Destroy(gameObject);
    }

    public void setCarName(string name)
    {
        this.name = name;
   
    }

    public void setSpawn(int spawn)
    {
        spawnPos = spawn;
    }
}
