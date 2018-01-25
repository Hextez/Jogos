using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour {

    public float boostPower = 16000f;
    public float normalPower = 8000f;
    public float delay = 4f;

    float countdown;
    public Rigidbody car;


    // Use this for initialization
    void Start () {
        countdown = delay;
        car.GetComponent<HoverCarControl>().forwardAcceleration = boostPower;
	}
	
	// Update is called once per frame
	void Update () {
        countdown -= Time.deltaTime;
        if (countdown <= 0f )
        {
            car.GetComponent<HoverCarControl>().forwardAcceleration = normalPower;
        }
    }
}
