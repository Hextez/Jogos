using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
public class HoverCarControl : NetworkBehaviour
{
  	Rigidbody body;
  	float deadZone = 0.1f;
	public float groundedDrag = 3f;
	public float maxVelocity = 100;
  	public float hoverForce = 1000;
	public float gravityForce = 1000f;
  	public float hoverHeight = 1.5f;
  	public GameObject[] hoverPoints;

    //Para iniciar e quando leva dano
    public bool canMove = true;

    //Lista dos poderes que existem e os poderes do jogador
    public Rigidbody[] powers;
    public static Rigidbody[] atualPower = new Rigidbody[2];

    //Posições para os poderes 
    public GameObject spawnPos;
    public GameObject spawnPosBack;
    public GameObject spawnPosMiddle;

    //Game objects dos poderes;
    public Rigidbody mine;
    public Rigidbody missile;
    public Rigidbody shield;

    //ranks
    public Text ranking;

    //velocidades
  	public float forwardAcceleration = 500000f;
  	public float reverseAcceleration = 4000f;
  	public float thrust = 0f;

 	public float turnStrength = 1000f;
  	float turnValue = 0f;

	public ParticleSystem[] dustTrails = new ParticleSystem[2];

 	int layerMask;

      void Start()
      {
        if(GameObject.Find("Camera")) //camera inicial
            GameObject.Find("Camera").SetActive(false);

        body = GetComponent<Rigidbody>();
	    body.centerOfMass = Vector3.down;

        layerMask = 1 << LayerMask.NameToLayer("Vehicle");
        layerMask = ~layerMask;

        ranking = GameObject.Find("Canvas").GetComponentInChildren<Text>();
      }

	
  void Update() //movimentos do carro
  {
        if (!isLocalPlayer) //so o local pode mexer 
        {
            return;
        }
        RaycastHit hit;
        float theDistance;

        Vector3 foward = transform.TransformDirection(Vector3.down);
        Debug.DrawRay(transform.position, foward, Color.green);

        //Alguma coisa esta a fuder com o canMove xD
        canMove = true;

        // Get thrust input
        if (canMove)
        {
            if (Physics.Raycast(transform.position, (foward), out hit))
            {
                theDistance = hit.distance;
            }
            thrust = 0.0f;
            float acceleration = 0;
            float turnAxis = 0;

            acceleration = Input.GetAxis("Vertical");
            turnAxis = Input.GetAxis("Horizontal");
                
            if (Input.GetButtonDown("FireP1"))
            {
                if (hit.collider.gameObject.name == "Cube_001" || hit.collider.gameObject.name == "Cube_003")
                {
                    if (atualPower[0] != null && atualPower[0].gameObject.name == "Missile")
                    {
                        if (name != GameObject.Find("CheckPoints").GetComponent<PositionTrack>().getFristPlace())
                        {
                            Homing info = missile.GetComponent<Homing>();

                            GameObject carTarget = GameObject.Find(GameObject.Find("CheckPoints").GetComponent<PositionTrack>().getFristPlace());

                            info.setTransform(carTarget.transform);
                            Instantiate(missile, spawnPos.transform.position, spawnPos.transform.rotation);
                            atualPower[0] = null;
                        }
                    }else if(atualPower[0] != null && atualPower[0].gameObject.name == "Mina") {
                        Instantiate(mine, spawnPosBack.transform.position, spawnPosBack.transform.rotation);
                        atualPower[0] = null;
                    }
                    else if (atualPower[0] != null && atualPower[0].gameObject.name == "Shield")
                    {
                        shield.GetComponent<Protector>().setPosPosition(spawnPosMiddle);
                        Instantiate(shield, spawnPosMiddle.transform.position, spawnPosMiddle.transform.rotation);
                        atualPower[0] = null;
                    }
                }
                else if(hit.collider.gameObject.name == "Cube_002" || hit.collider.gameObject.name == "Cube_004")
                {
                    if (atualPower[1] != null && atualPower[1].gameObject.name == "Missile")
                    {
                        if (name != GameObject.Find("CheckPoints").GetComponent<PositionTrack>().getFristPlace())
                        {
                            Homing info = missile.GetComponent<Homing>();

                            GameObject carTarget = GameObject.Find(GameObject.Find("CheckPoints").GetComponent<PositionTrack>().getFristPlace());

                            info.setTransform(carTarget.transform);
                            Instantiate(missile, spawnPos.transform.position, spawnPos.transform.rotation);
                            atualPower[1] = null;
                        }
                    }
                    else if (atualPower[1] != null && atualPower[1].gameObject.name == "Mina")
                    {
                        Instantiate(mine, spawnPosBack.transform.position, spawnPosBack.transform.rotation);
                        atualPower[1] = null;
                    }
                    else if (atualPower[1] != null && atualPower[1].gameObject.name == "Shield")
                    {
                        shield.GetComponent<Protector>().setPosPosition(spawnPosMiddle);
                        Instantiate(shield, spawnPosMiddle.transform.position, spawnPosMiddle.transform.rotation);
                        atualPower[1] = null;
                    }
                }
                
            }
           
            if (acceleration > deadZone)
                thrust = acceleration * forwardAcceleration;
            else if (acceleration < -deadZone)
                thrust = acceleration * reverseAcceleration;

            // Get turning input
            turnValue = 0.0f;

            if (Mathf.Abs(turnAxis) > deadZone)
            {
                if (acceleration < 0)
                    turnValue = -turnAxis;
                else
                    turnValue = turnAxis;
            }
        }
  }

  void FixedUpdate()
  {

    //  Do hover/bounce force
		RaycastHit hit;
		bool  grounded = false;
    	for (int i = 0; i < hoverPoints.Length; i++)
    	{
      		var hoverPoint = hoverPoints [i];
			if (Physics.Raycast(hoverPoint.transform.position, -Vector3.up, out hit,hoverHeight, layerMask))
			{
				body.AddForceAtPosition(Vector3.up * hoverForce* (1.0f - (hit.distance / hoverHeight)), hoverPoint.transform.position);
				grounded = true;
			}
      		else
      		{
				// Self levelling - returns the vehicle to horizontal when not grounded and simulates gravity
		        if (transform.position.y > hoverPoint.transform.position.y)
				{
					body.AddForceAtPosition(hoverPoint.transform.up * gravityForce, hoverPoint.transform.position);
				}
		        else
				{
					body.AddForceAtPosition(hoverPoint.transform.up * -gravityForce, hoverPoint.transform.position);
				}
      		}
    	}
			
		var emissionRate = 0;
		if(grounded)
		{
			body.drag = groundedDrag;
			emissionRate = 10;
		}
		else
		{
			body.drag = 0.1f;
			thrust /= 100f;
			turnValue /= 100f;
		}

		for(int i = 0; i<dustTrails.Length; i++)
		{
			var emission = dustTrails[i].emission;
			emission.rate = new ParticleSystem.MinMaxCurve(emissionRate);
		}

        // Handle Forward and Reverse forces
        if (Mathf.Abs(thrust) > 0)
          body.AddForce(transform.forward * thrust);

        // Handle Turn forces
        if (turnValue > 0)
        {
          body.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
        } else if (turnValue < 0)
        {
          body.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
        }

	    // Limit max velocity
	    if(body.velocity.sqrMagnitude > (body.velocity.normalized * maxVelocity).sqrMagnitude)
	    {
		    body.velocity = body.velocity.normalized * maxVelocity;
	    }
      }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            //other.gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            for (int x = 0; x < atualPower.Length; x++)
            {
                if (x == 0 && atualPower[x] == null)
                {
                    atualPower[x] = powers[UnityEngine.Random.Range(0, powers.Length)];
                }
                else if (x == 1 && atualPower[x] == null)
                {
                    atualPower[x] = powers[UnityEngine.Random.Range(0, powers.Length)];
                }
            }

            StartCoroutine(Wait(other.gameObject));
        }
    }

    IEnumerator Wait(GameObject bo)
    {
        //print(Time.time);
        yield return new WaitForSeconds(5);
        bo.SetActive(true);
    }

}
