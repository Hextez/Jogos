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
    [SyncVar (hook="setMove")] public bool canMove = false;
    [SyncVar (hook="setProtected")] public bool pprotected = false;
    public bool wrongWay = false;

    //Lista dos poderes que existem e os poderes do jogador
    public GameObject[] powers;
    public Sprite[] powersImgs;
    private int[] primeiroL = new int[8];
    private int[] ultimoL = new int[6];
    public int[] atualPower = new int[2];

    //Posições para os poderes 
    public GameObject spawnPos;
    public GameObject spawnPosBack;
    public GameObject spawnPosMiddle;
    public GameObject spawnPosLeft;
    public GameObject spawnPosRight;


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

    //Info para o boost
    public float delay = 4f;
    float countdown;

    //Contador da metralha
    int balas = 3;

    //Pause do jogo
    public Transform canvasPause;

	public ParticleSystem[] dustTrails = new ParticleSystem[2];

 	int layerMask;

    public bool fire { get; private set; }
    public bool r1 { get; private set; }
    public bool l1 { get; private set; }

    public GameObject warning;
    public bool respawn = false;

    void Start()
      {
        if (GameObject.Find("player"))
            gameObject.name = "player1";

        if(GameObject.Find("Camera")) //camera inicial
            GameObject.Find("Camera").SetActive(false);

        body = GetComponent<Rigidbody>();
	    body.centerOfMass = Vector3.down;

        layerMask = 1 << LayerMask.NameToLayer("Vehicle");
        layerMask = ~layerMask;

        ranking = GameObject.Find("Canvas").GetComponentsInChildren<Text>()[0];
        
        spawnPosMiddle = gameObject.transform.Find("SpawnPointMiddle").gameObject;
        spawnPos = gameObject.transform.Find("SpawnPoint").gameObject;
        spawnPosBack = gameObject.transform.Find("SpawnPointBack").gameObject;
        spawnPosLeft = gameObject.transform.Find("SpawnPointLeft").gameObject;
        spawnPosRight = gameObject.transform.Find("SpawnPointRight").gameObject;

        //boost = new GameObject();
        countdown = delay;
        atualPower[0] = -1;
        atualPower[1] = -1;

        primeiroL[0] = 0;
        primeiroL[1] = 1;
        primeiroL[2] = 2;
        primeiroL[3] = 3;
        primeiroL[4] = 4;
        primeiroL[5] = 5;
        primeiroL[6] = 6;
        primeiroL[7] = 11;

        ultimoL[0] = 4;
        ultimoL[1] = 5;
        ultimoL[2] = 6;
        ultimoL[3] = 7;
        ultimoL[4] = 8;
        ultimoL[5] = 9;

        

        if (isLocalPlayer)
        {
            warning = GameObject.FindGameObjectWithTag("WWTAG");
            warning.SetActive(false);
        }


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

        if (atualPower[0] != -1)
        {
            GameObject.Find("GBImage").GetComponent<Image>().sprite = powersImgs[atualPower[0]];
        }
        else
        {
            GameObject.Find("GBImage").GetComponent<Image>().sprite = powersImgs[powersImgs.Length-1];

        }
        if (atualPower[1] != -1)
        {
            GameObject.Find("RYImage").GetComponent<Image>().sprite = powersImgs[atualPower[1]];

        }
        else
        {
            GameObject.Find("RYImage").GetComponent<Image>().sprite = powersImgs[powersImgs.Length - 1];

        }

        RaycastHit hit;
        float theDistance;

        Vector3 foward = transform.TransformDirection(Vector3.down);
        Debug.DrawRay(transform.position, foward, Color.green);
        Debug.Log(canMove);
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
            if (Input.GetAxis("LeftJoySticjHorizontal") == 0)
            {
                turnAxis = Input.GetAxis("Horizontal");
            }
            else
            {
                turnAxis = Input.GetAxis("LeftJoySticjHorizontal");
            }
            

            if (hit.collider.gameObject.name == "Terrain")
            {
                forwardAcceleration = 5000f;
            }
            else
            {
                forwardAcceleration = 25000f;
            }
            
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
                        setProtected(true);
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
                    else if (atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "Bala")
                    {

                        CmdMetralha(atualPower[0]);
                        fire = false;
                        if (balas > 1) { 
                            balas--;
                        }
                        else {
                            balas = 3;
                            atualPower[0] = -1;
                        }
                    }
                    else if (atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "STOP")
                    {
                        CmdSign(atualPower[0]);
                        atualPower[0] = -1;
                    }
                    else if (atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "Field")
                    {
                        CmdForceField(atualPower[0]);
                        setProtected(true);
                        atualPower[0] = -1;
                    }
                    else if (atualPower[0] != -1 && powers[atualPower[0]].gameObject.name == "Exit")
                    {
                        CmdTeletransport(atualPower[0]);
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
                        setProtected(true);
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
                    else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "Bala")
                    {
                        CmdMetralha(atualPower[1]);
                        fire = false;
                        if (balas > 1)
                        {
                            balas--;
                        }
                        else
                        {
                            balas = 3;
                            atualPower[1] = -1;
                        }
                    }
                    else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "STOP")
                    {
                        CmdSign(atualPower[1]);
                        atualPower[1] = -1;
                    }
                    else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "Field")
                    {
                        setProtected(true);
                        CmdForceField(atualPower[1]);
                        atualPower[1] = -1;
                    }
                    else if (atualPower[1] != -1 && powers[atualPower[1]].gameObject.name == "Exit")
                    {
                        CmdTeletransport(atualPower[1]);
                        atualPower[1] = -1;
                    }
                }
            }

            if (acceleration > deadZone)
            {
                thrust = acceleration * forwardAcceleration;
                if(wrongWay)
                {
                    warning.SetActive(true);
                } else if(warning.activeInHierarchy)
                {
                    warning.SetActive(false);
                }
            }
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
        pos[1] = pos[1]+4; // the Z value
        GameObject bullet = (GameObject)Instantiate(powers[val], pos, spawnPosMiddle.transform.rotation);
        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdOil(int val)
    {

        Vector3 pos = spawnPosBack.transform.position;
        pos[2] = pos[2] + 5; // the Z value

        GameObject bullet = (GameObject)Instantiate(powers[val], pos, Quaternion.Euler(spawnPosBack.transform.rotation.x + 90, spawnPosBack.transform.rotation.y, spawnPosBack.transform.rotation.z));
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

    [Command]
    void CmdMetralha(int val)
    {
        Vector3 pos = spawnPos.transform.position;

        pos[2] = pos[2] - 3;
        GameObject bullet = (GameObject)Instantiate(powers[val],pos, spawnPos.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 10000f);
        NetworkServer.Spawn(bullet);
    }
    [Command]
    void CmdSign(int val)
    {
        Vector3 pos = spawnPosBack.transform.position;
        pos[2] = pos[2] + 5; // the Z value
        pos[1] = pos[1] + 2;
        GameObject bullet = (GameObject)Instantiate(powers[val], pos+(transform.forward*-1), Quaternion.Euler(spawnPosBack.transform.rotation.eulerAngles.x, spawnPosBack.transform.rotation.eulerAngles.y + 90, spawnPosBack.transform.rotation.eulerAngles.z));
        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdTeletransport(int val)
    {
        Quaternion rotation = Quaternion.Euler(spawnPosBack.transform.rotation.eulerAngles.x + 90, spawnPosBack.transform.rotation.eulerAngles.y, spawnPosBack.transform.rotation.eulerAngles.z);
        GameObject bullet = (GameObject)Instantiate(powers[val], spawnPosBack.transform.position, rotation);
        bullet.GetComponent<TeletransportScript>().setCar(gameObject.name);
        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdForceField(int val)
    {
        GameObject bullet = (GameObject)Instantiate(powers[val], spawnPosMiddle.transform.position, spawnPosMiddle.transform.rotation);
        bullet.GetComponent<FieldEffect>().setCar(gameObject.name);
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

                    atualPower[x] = UnityEngine.Random.Range(0, powers.Length + 1);
                    

                }
                else if (x == 1 && atualPower[x] == -1)
                {
                    atualPower[x] = UnityEngine.Random.Range(0, powers.Length + 1);

                }
            }

            StartCoroutine(Wait(other.gameObject));
        }
        if (other.gameObject.CompareTag("Out"))
        {


            Transform infoCheck = GetComponent<CarCheckpoint>().checkPointArray[GetComponent<CarCheckpoint>().getCurrentCheck() - 1].transform;
            transform.rotation = Quaternion.Euler(infoCheck.rotation.eulerAngles);
            Vector3 pos = infoCheck.position+ transform.forward * 10;
            transform.position = pos;
            thrust = 0f;
            

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
    public void setProtected(bool b)
    {
        pprotected = b;
    }
    public void setWW(bool b)
    {
        wrongWay = b;
    }
}
