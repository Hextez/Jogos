using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour {

    public Text count;
    private float startTime;
    public HoverCarControl player1;
    public HoverCarControl player2;

    // Use this for initialization
    void Start () {
        startTime = 3;
    }


    // Update is called once per frame
    void Update()
    {

        Debug.Log("CountDOw");
        float t = startTime - Time.time;
        if (t > -1) { 
            string seconds = (t % 60).ToString("f2");

            count.text = seconds;
// Debug.Log(t);
            if (t < 0)
            {
                count.text = "GO!!!";
                player1.canMove = true;
                player2.canMove = true;
            }
        }
        if (t < -1)
        {
            Destroy(count);
            
        }
    }
}
