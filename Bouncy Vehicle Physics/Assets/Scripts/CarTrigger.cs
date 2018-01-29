﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class CarTrigger : NetworkBehaviour {

    private bool damageAnimation = true;
    private Material mat;
    private Color[] colors = { Color.yellow, Color.red };
    public HoverCarControl player;
    public CarCheckpoint car1;

    void Start()
    {
        player = gameObject.GetComponent<HoverCarControl>();
        car1 = gameObject.GetComponent<CarCheckpoint>();

    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Oil"))
        {
            gameObject.GetComponent<HoverCarControl>().forwardAcceleration = 25000f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer)
            return;
        
        if (other.CompareTag("Explosion"))
        {
            gameObject.GetComponent<Rigidbody>().AddExplosionForce(100000f, other.transform.position, 500f);
        }
        if (other.CompareTag("Oil"))
        {
            gameObject.GetComponent<HoverCarControl>().thrust = 0f ;
            gameObject.GetComponent<HoverCarControl>().forwardAcceleration = 0f;
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -260000f);
        }

        if ((other.CompareTag("Missile") && other.GetComponent<Homing>().name == gameObject.name) || other.CompareTag("Mina"))
        {
            
            player.setMove(false);
            damageAnimation = false;
            player.thrust = 0f;
            Transform childtoWork = null;
            foreach (Transform child in transform)
            {
                if (child.name == "Model")
                {
                    childtoWork = child.transform.GetChild(0);
                    mat = childtoWork.GetComponent<Renderer>().material;
                }
            }
            StartCoroutine(Flash(0.4f, 0.05f));
            new WaitForSeconds(3);
        }

        if (!other.CompareTag("Check"))
            return;

        //Laps e check
        CarCheckpoint car1 = gameObject.GetComponent<CarCheckpoint>();
        //Debug.Log(other.name + ".----. " + car1.checkPointArray[car1.getCurrentCheck()].name);
        //Is this transform equal to the transform of checkpointArrays[currentCheckpoint]?
        if (other.transform == car1.checkPointArray[car1.getCurrentCheck()].transform)
        {
            //Check so we dont exceed our checkpoint quantity
            if (car1.getCurrentCheck() + 1 < car1.checkPointArray.Count)
            {
                //Add to currentLap if currentCheckpoint is 0
                if (car1.getCurrentCheck() == 0)
                    car1.setLap(car1.getCurrentLap() + 1);
                car1.setCheck(car1.getCurrentCheck() + 1);
            }
            else
            {
                //If we dont have any Checkpoints left, go back to 0
                car1.setCheck(0);
            }
            //VisualAid(); //Run a coroutine to update the visual aid of our Checkpoints
            //Update the 3dtext
        }
        //Debug.Log(car1.getCurrentCheck());
    }



    IEnumerator Flash(float time, float intervalTime)
    {
        float elapsedTime = 0f;
        int index = 0;
        while (elapsedTime < time)
        {
            mat.color = colors[index % 2];

            elapsedTime += Time.deltaTime;
            index++;
            yield return new WaitForSeconds(intervalTime);
        }

        player.setMove(true);
        damageAnimation = true;
    }

}
