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
    [SyncVar (hook="setMove")] public bool canMove = true;

    //Lista dos poderes que existem e os poderes do jogador
    public GameObject[] powers;
    public int[] atualPower = new int[2];

    //Posições para os poderes 
    public GameObject spawnPos;
    public GameObject spawnPosBack;
    public GameObject spawnPosMiddle;
    public GameObject spawnPosLeft;
    public GameObject spawnPosRight;

    //Game objects dos poderes;
    // public GameObject mine;
    //public GameObject missile;
    //public GameObject shield;
    //public GameObject bomb;
    //public GameObject oil;
    //public GameObject boost;

    //ranks
    public Text ranking;
    public Text textPower;

    //velocidades
    public float boostPower = 35000f;
    public float forwardAcceleration = 500000f;
  	public float reverseAcceleration = 4000f;
  	public float thrust = 0f;

 	public float turnStrength = 1000f;
  	float turnValue = 0f;

    public float delay = 4f;
    float countdown;


	public ParticleSystem[] dustTrails = new ParticleSystem[2];

 	int layerMask;

    public bool fire { get; private set; }
    public bool r1 { get; private set; }
    public bool l1 { get; private set; }

    void Start()
      {
        if(GameObject.Find("Camera")) //camera inicial
            GameObject.Find("Camera").SetActive(false);

        body = GetComponent<Rigidbody>();
	    body.centerOfMass = Vector3.down;

        layerMask = 1 << LayerMask.NameToLayer("Vehicle");
        layerMask = ~layerMask;

        ranking = GameObject.Find("Canvas").GetComponentsInChildren<Text>()[0];
        textPower = GameObject.Find("Canvas").GetComponentsInChildren<Text>()[1];
        textPower.text = "";

        spawnPosMiddle = gameObject.transform.Find("SpawnPointMiddle").gameObject;
        spawnPos = gameObject.transform.Find("SpawnPoint").gameObject;
        spawnPosBack = gameObject.transform.Find("SpawnPointBack").gameObject;
        spawnPosLeft = gameObject.transform.Find("SpawnPointLeft").gameObject;
        spawnPosRight = gameObject.transform.Find("SpawnPointRight").gameObject;

    //boost = new GameObject();
    countdown = delay;
        atualPower[0] = -1;
        atualPower[1] = -1;
    }


    void Update() //movimentos do carro
    {
        if (isServer)
        {
            if (GameObject.Find("BigExplosionEffect(Clone)"))
            {
                DestroyObject(GameObject.Find("BigExplosionEffect(Clone)"), 2f);

            }
        }
        if (!isLocalPlayer) //so o local pode mexer 
        {
            return;
        }

        if (forwardAcceleration == 35000f) { 
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                forwardAcceleration = 25000f;
            }
        }

        string p = "";
        foreach (int obj in atualPower)
        {
            Debug.Log(obj);
            if (obj != -1)
            {
                if (obj == powers.Length)
                {
                    p += "Boost";
                }
                else
                {
                    p += powers[obj].name;
                }
            }
            else
                p += "Nada";
        }
        textPower.text = p;

        RaycastHit hit;
        float theDistance;

        Vector3 foward = transform.TransformDirection(Vector3.down);
        Debug.DrawRay(transform.position, foward, Color.green);

        if (canMove)
        {

            if (Physics.Raycast(transform.position, (foward), out hit))
            {
                theDistance = hit.distance;
            }
            thrust = 0.0f;
            float acceleration = 0;
            float turnAxis = 0;
            buttonsDown();
            buttonsUp();
            acceleration = Input.GetAxis("Vertical");
            turnAxis = Input.GetAxis("Horizontal");           
            if (fire)
            {
                if (hit.collider.gameObject.name == "Verde" || hit.collider.gameObject.name == "Azul")
                {
                    if (atualPower[0] != -1 && powers.Length == atualPower[0])
                    {
                        forwardAcceleration = boostPower;
                        atualPower[0] = -1;
                    }
                    else if (atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "Missile")
                    {
                        if (name != gameObject.GetComponent<PositionTrack>().getFristPlace())
                        {
                            CmdShootMissile(atualPower[0]);
                            atualPower[0] = -1;
                        }
                    }
                    else if (atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "Mina")
                    {

                        CmdDropMine(atualPower[0]);
                        atualPower[0] = -1;
                    }
                    else if (atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "Shield")
                    {

                        CmdShield(atualPower[0]);
                        atualPower[0] = -1;
                    }
                    else if (atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "Bomba")
                    {

                        CmdBomb(atualPower[0]);
                        atualPower[0] = -1;
                    }else if (atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "Liquido")
                    {
                        CmdOil(atualPower[0]);
                        atualPower[0] = -1;
                    }
                    else if(atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "Punch")
                    {
                        CmdPunch(atualPower[0], r1, l1);
                        atualPower[0] = -1;
                    }
                }
                else if (hit.collider.gameObject.name == "Amarelo" || hit.collider.gameObject.name == "Vermelho")
                {
                    if (atualPower[1] != -1 && powers.Length == atualPower[1])
                    {
                        forwardAcceleration = boostPower;
                        atualPower[1] = -1;
                    }else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "Missile")
                    {
                        if (name != gameObject.GetComponent<PositionTrack>().getFristPlace())
                        {

                            CmdShootMissile(atualPower[1]);
                            atualPower[1] = -1;
                        }
                    }
                    else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "Mina")
                    {
                        CmdDropMine(atualPower[1]);
                        atualPower[1] = -1;
                    }
                    else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "Shield")
                    {

                        CmdShield(atualPower[1]);
                        atualPower[1] = -1;
                    }
                    else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "Bomba")
                    {

                        CmdBomb(atualPower[1]);
                        atualPower[1] = -1;
                    }
                    else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "Liquido")
                    {
                        CmdOil(atualPower[1]);
                        atualPower[1] = -1;
                    }
                    else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "Punch")
                    {
                        CmdPunch(atualPower[1], r1, l1);
                        atualPower[1] = -1;
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

    private void buttonsUp()
    {
        if (Input.GetButtonUp("FireP1"))
        {
            fire = false;
        }
        if (Input.GetButtonUp("R1"))
        {
            r1 = false;
        }
        if (Input.GetButtonUp("L1"))
        {
            l1 = false;
        }
    }

    private void buttonsDown()
    {
        if(Input.GetButtonDown("FireP1"))
        {
            fire = true;
        }
        if(Input.GetButtonDown("R1"))
        {
            r1 = true;
        }
        if(Input.GetButtonDown("L1"))
        {
            l1 = true;
        }
       
    }

    [Command]
    void CmdShootMissile(int val)
    {
        GameObject bullet = (GameObject)Instantiate(powers[val], spawnPos.transform.position, spawnPos.transform.rotation);
        Debug.Log(gameObject.GetComponent<PositionTrack>().getFristPlace());
        bullet.GetComponent<Homing>().setTargetName(gameObject.GetComponent<PositionTrack>().getFristPlace());
        NetworkServer.SpawnWithClientAuthority(bullet, gameObject);
    }

    [Command]
    void CmdDropMine(int val)
    {
        GameObject bullet = (GameObject)Instantiate(powers[val], spawnPosBack.transform.position, spawnPosBack.transform.rotation);
        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdShield(int val)
    {
        GameObject bullet = (GameObject)Instantiate(powers[val], spawnPosMiddle.transform.position, spawnPosMiddle.transform.rotation);
        Debug.Log(gameObject.name);
        bullet.GetComponent<Protector>().setPosPosition(gameObject.name);
        NetworkServer.SpawnWithClientAuthority(bullet, gameObject);
    }

    [Command]
    void CmdBomb(int val)
    {
        Vector3 pos = spawnPosMiddle.transform.position;
        pos[1] = pos[1]+2; // the Z value

        GameObject bullet = (GameObject)Instantiate(powers[val], pos, spawnPosMiddle.transform.rotation);
        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdOil(int val)
    {
        Vector3 pos = spawnPosBack.transform.position;
        pos[2] = pos[2] + 5; // the Z value

        GameObject bullet = (GameObject)Instantiate(powers[val], pos, Quaternion.Euler(spawnPosBack.transform.rotation.x +90, spawnPosBack.transform.rotation.y, spawnPosBack.transform.rotation.z));
        NetworkServer.Spawn(bullet);
    }
    [Command]
    void CmdPunch(int val, bool r1, bool l1)
    {
        int pos = 1;
        Vector3 position = spawnPos.transform.position;
        Quaternion rotation = Quaternion.Euler(spawnPos.transform.rotation.eulerAngles.x + 90, spawnPos.transform.rotation.eulerAngles.y, spawnPos.transform.rotation.eulerAngles.z);
        if (r1)
        {
            rotation = Quaternion.Euler(spawnPosRight.transform.rotation.eulerAngles.x + 90, spawnPos.transform.rotation.eulerAngles.y+90, spawnPos.transform.rotation.eulerAngles.z);
            pos = 2;
            position = spawnPosRight.transform.position;
        }
        else if(l1)
        {
            rotation = Quaternion.Euler(spawnPosLeft.transform.rotation.eulerAngles.x + 90, spawnPos.transform.rotation.eulerAngles.y-90, spawnPos.transform.rotation.eulerAngles.z);
            pos = 3;
            position = spawnPosLeft.transform.position;
        } 
        GameObject bullet = (GameObject)Instantiate(powers[val], position, rotation);
        bullet.GetComponent<StringEffect>().setCarName(gameObject.name);
        bullet.GetComponent<StringEffect>().setSpawn(pos);
        NetworkServer.SpawnWithClientAuthority(bullet, gameObject);
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
        {
            body.AddForce(transform.forward * thrust);
            
        }
          

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
        if (!isLocalPlayer)
            return;

        if (other.gameObject.CompareTag("PickUp"))
        {
            //other.gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            
            for (int x = 0; x < atualPower.Length; x++)
            {
                if (x == 0 && atualPower[x] == -1)
                {
                    
                    //atualPower[x] = powers[UnityEngine.Random.Range(0, powers.Length)];
                    atualPower[x] = 5;

                }
                else if (x == 1 && atualPower[x] == -1)
                {
                    //atualPower[x] = powers[UnityEngine.Random.Range(0, powers.Length)];
                    atualPower[x] = 5;
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

    public void setMove(bool b)
    {
        canMove = b;
    }

}
