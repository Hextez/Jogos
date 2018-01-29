using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class PositionTrack : NetworkBehaviour {

    public GameObject[] players;
    public GameObject[] playersInPos;

    //public GameObject[] players;

    public Text ranking;
    public Text textPlaye1;

    [SyncVar(hook = "setRanks")] public string texto;

    private CarCheckpoint car;


    // Use this for initialization
    void Start () {
        
        ranking = GameObject.Find("Canvas").GetComponentsInChildren<Text>()[0];
        textPlaye1 = GameObject.Find("Canvas").GetComponentsInChildren<Text>()[3];

        players = GameObject.FindGameObjectsWithTag("Player1");
        playersInPos = players;

        car = GetComponent<CarCheckpoint>();
    }

    // Update is called once per frame
    void Update () {
       
        textPlaye1.text = "Checkpoint: " + (car.getCurrentCheck()) + " Lap: " + (car.getCurrentLap());

        players = GameObject.FindGameObjectsWithTag("Player1");
        if (players.Length != playersInPos.Length)
            playersInPos = players;

        int x = 0;
        if (playersInPos.Length >= 1)
        {
            while (x < playersInPos.Length - 1)
            {
                GameObject obj = CheckLaps(playersInPos[x],playersInPos[x + 1]);
                
                if (obj.name == playersInPos[x + 1].name)
                {
                    GameObject oldObj = playersInPos[x];
                    GameObject newObj = obj;
                    
                    int oldInd = System.Array.IndexOf(playersInPos, oldObj);
                    int newInd = System.Array.IndexOf(playersInPos, newObj);

                    playersInPos.SetValue(newObj, oldInd);
                    playersInPos.SetValue(oldObj, newInd);
                    
                    x = 0;
                }
                x++;
            }
        }
        string tex = "";
        foreach (GameObject ob in playersInPos)
        {
            tex += ob.name + " ";
        }

        setRanks(tex);
        ranking.text = tex;

	}

    public void setRanks(String s)
    {
        texto = s;
    }

    public string getFristPlace()
    {
        //Debug.Log(texto+ "dsdsdsds");
        return GameObject.Find("Canvas").GetComponentsInChildren<Text>()[0].text.Split(new string[] { " " }, StringSplitOptions.None)[0];

    }

    GameObject DistanceBetweenCheck(GameObject obj1, GameObject obj2)
    {
        CarCheckpoint fromCar = obj1.GetComponent<CarCheckpoint>();
        Transform check = fromCar.checkPointArray[fromCar.getCurrentCheck()].transform;

        if (Vector3.Distance(obj1.transform.position,check.position) > Vector3.Distance(obj2.transform.position, check.position)){
            return obj2;
        }else if (Vector3.Distance(obj1.transform.position, check.position) < Vector3.Distance(obj2.transform.position, check.position)){
            return obj1;
        }
        else
        {
            return obj1;
        }

    }

    GameObject CheckCheckpoints(GameObject obj1, GameObject obj2)
    {
        CarCheckpoint fromCar1 = obj1.GetComponent<CarCheckpoint>();
        CarCheckpoint fromCar2 = obj2.GetComponent<CarCheckpoint>();
        if (fromCar1.getCurrentCheck() == 0 || fromCar2.getCurrentCheck() == 0) //Quando nao existe mais checkpoints ele vai para 0
        {
            if (fromCar1.getCurrentCheck() < fromCar2.getCurrentCheck())
            {
                return obj1;
            }else if (fromCar1.getCurrentCheck() > fromCar2.getCurrentCheck())
            {
                return obj2;
            }
            else
            {
                return DistanceBetweenCheck(obj1, obj2);
            }
        }
        else
        {
            if (fromCar1.getCurrentCheck() > fromCar2.getCurrentCheck())
            {
                return obj1;
            }
            else if (fromCar1.getCurrentCheck() < fromCar2.getCurrentCheck())
            {
                return obj2;
            }
            else
            {
                return DistanceBetweenCheck(obj1,obj2);
            }
        }
    }

    GameObject CheckLaps(GameObject obj1, GameObject obj2)
    {
        CarCheckpoint fromCar1 = obj1.GetComponent<CarCheckpoint>();
        CarCheckpoint fromCar2 = obj2.GetComponent<CarCheckpoint>();
        //Debug.Log(fromCar1.getCurrentLap() + fromCar1.name + "Lap");
        //Debug.Log(fromCar2.getCurrentLap() + fromCar2.name + "Lap");
        if (fromCar1.getCurrentLap() > fromCar2.getCurrentLap())
        {
            return obj1;
        }else if (fromCar1.getCurrentLap() < fromCar2.getCurrentLap())
        {
            return obj2;
        }
        else
        {
            return CheckCheckpoints(obj1,obj2);
        }
    }


}
