using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	private NetworkPlayer m_Owner;
	
	public List<GameObject> m_Guards;
	public List<GameObject> m_Characters;
	private GameObject m_CurrentChar;
	
	private float m_LastClientHInput = 0.0f;
	private float m_CurrentServerHInput = 0.0f;
	
	private float m_LastClientSInput = 0.0f;
	private float m_CurrentServerSInput = 0.0f;
	private float m_ShootAnimationLengthInSeconds = 0.2f;
	private float m_ShootAnimSpeed = 3.0f;
	private float m_ShootAnimTimer = 0.0f;
	
	public int m_PlayerController = 0;
	public float m_ShootDelay = 0.3f;
	public float m_ShootDeviation = 130.0f;//100.0f;//10.0f; //THIS IS CHANGED IN THE INSPECTOR
	
	private int m_OtherPlayer = 0;
	
	float playerSpeed = Globals.g_PlayerSpeed;
	int xLimit = (int)Globals.g_PlayerXLimit;
	
	private Vector3 m_InitialPosition = new Vector3(0,0,0);
	
	private Ball m_HeldBall = null;
	private bool b_SameButtonShootBuffer = false;
	private bool b_HoldingBall = false;
	private float m_ShootTimer = 0.0f;
	private float m_StunTime = 0;
	private float m_MaxShootDistance = 25;
	private int m_BallsOnSide = 0;//This is how many balls are on the player side, if they have all of them they lose
	private int m_TotalBalls = 0;
	private Ball.Sides m_MySide = Ball.Sides.BOTTOM;
	private Color m_MyColor = new Color(1.0f,1.0f,1.0f,1.0f);
	private float m_Radius = 0;
	private bool b_SpecialAvailable = false;
	private Vector3 m_Velocity = Vector3.zero;
	private float m_ServerStunTime = 0;
	private float m_ChargeShootTimer = 0.0f;
	private const float m_MaxChargeTime = 0.3f;
	private bool b_Charging = false;
	private bool b_Charged = false;
	private int m_CurrentCharacter = 0;
	private Vector3 aimPos;
	
	private float m_DeadZone = 0.05f;
	
	public bool m_EndedTheGame = false;
	
	private bool m_PowerShot = true;
	
	private int m_Special = -1;
	
	private float m_ServerH = 0;
	
	private int m_SetToStun = -2;
	
	Transform m_HandTransform = null;
	
	Transform m_HeadTransform = null;
	
	private float m_THEDANGERZONE;
	private Camera m_Camera;
	
	public GameObject m_StunParticle;
	
	//Array of all the balls
	//private GameObject[] m_BallObjects;
	
	public GameObject m_FireBlue;
	public GameObject m_FireRed;
	
	private GameObject[] m_FireBlueList;
	private GameObject[] m_FireRedList;

	public GameObject m_FireStand;
	private GameObject[] m_FireStandList;
	
	private bool m_Paused = false;

	public GameObject aimObject;

	private bool m_EndGame = false;
	
	public enum SoundEffects
	{
		STEP1 = 0,
		STEP2,
		STEP3,
		STEP4,
		VICTORY,
		MAX	
	};
	
	public AudioClip[] AudioClips;// = new AudioSource[SoundEffects.MAX];
	
	public AudioSource Source;
	
	void Awake()
	{

	}
	
	public bool IsInputCanceled()
	{
		return m_Camera.IsCinematicPlaying();
	}
	
	[RPC]
	void SetPlayer(NetworkPlayer player)
	{
		SetOwner(player);
		//enable the script for the client
		//this enables the update function
		if (player == Network.player)
			enabled = true;
		
		Globals.g_Players = GameObject.FindGameObjectsWithTag("Player");
	}
	
	public void SetPaused(bool pause)
	{
		m_Paused = pause;
	}
	
	
	public void PlayStepSound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay =Random.Range((int)SoundEffects.STEP1, (int)SoundEffects.STEP4);
		Source.clip = AudioClips[soundToPlay];
		Source.Play();	
	}

	public void PlayVictorySound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay =(int)SoundEffects.VICTORY;
		Source.clip = AudioClips[soundToPlay];
		Source.Play();	
	}
	
	// Use this for initialization
	void Start () 
	{
		//Time will reset when a player gets created
		//Globals.g_Timer.GetComponent<TimerScript>().ResetTime();
	
		aimObject = (GameObject)Instantiate(aimObject);

		m_Camera = GameObject.FindWithTag("MainCamera").camera.GetComponent<Camera>();
		
		//GetComponent<MeshRenderer>().enabled = false;
		
		
		m_FireBlueList = new GameObject[3];
		m_FireRedList = new GameObject[3];

		m_FireStandList = new GameObject[6];
		
		Vector3 blueFirePos = new Vector3(-187.0f, 43.0f, 100.0f);//new Vector3(187.0f, 43.0f, -100.0f);
		
		Vector3 redFirePos = new Vector3(187.0f, 43.0f, -100.0f);//new Vector3(-187.0f, 43.0f, 100.0f);

		aimPos = Vector3.zero;

		for(int i = 0; i < 3; i++)
		{
			blueFirePos.z = 200.0f - (i*40);//-80.0f + (i*40);
			m_FireBlueList[i] = (GameObject)Instantiate(m_FireBlue,blueFirePos,transform.localRotation);
			m_FireBlueList[i].transform.localScale = new Vector3(0.8f,0.8f,0.8f);
			m_FireBlueList[i].SetActive(false);


			blueFirePos.y -= 18;
			m_FireStandList[i] = (GameObject)Instantiate(m_FireStand,blueFirePos,transform.localRotation);
			m_FireStandList[i].SetActive(true);
			blueFirePos.y += 18;
		}
		
		for(int i = 0; i < 3; i++)
		{	
			redFirePos.z = -80.0f + (i*40);//200.0f - (i*40);
			m_FireRedList[i] = (GameObject)Instantiate(m_FireRed, redFirePos,transform.localRotation );
			m_FireRedList[i].transform.localScale = new Vector3(0.8f,0.8f,0.8f);
			m_FireRedList[i].SetActive(false);

			redFirePos.y -= 18;
			m_FireStandList[i+3] = (GameObject)Instantiate(m_FireStand,redFirePos,transform.localRotation);
			m_FireStandList[i+3].SetActive(true);
			redFirePos.y += 18;
		}
		m_Paused = true;
		
		m_EndGame = false;
		

		
		if(m_PlayerController == 0)
		{
			this.transform.position = Globals.g_PlayerInitialPos[0];
			m_OtherPlayer = 1;
			m_MySide = Ball.Sides.BOTTOM;
			
		}else
		{
			this.transform.position = Globals.g_PlayerInitialPos[1];
			//this.transform.Rotate(0,180,0);
			m_OtherPlayer = 0;
			m_MySide = Ball.Sides.TOP;
			
		}
	
		m_Radius = 14;//transform.localScale.x/4;
		
	
		//m_BallObjects = Globals.g_Balls;
		m_TotalBalls = Globals.g_TotalBalls;
		
		m_InitialPosition = this.transform.position;
		
		m_MyColor = renderer.material.color;
		
		Globals.g_Players = GameObject.FindGameObjectsWithTag("Player");	
	}
	
	public void SetStunTime(float time)
	{
		m_StunTime = time;
	}
	
	public float GetStunTime()
	{
		return m_StunTime;
		
	}
	
	
	
	public void SetCharacter(int ID)
	{
		if(m_PlayerController == 1)
		{
			this.transform.Rotate(0,180,0);	
		}
		
		m_CurrentChar = (GameObject)Instantiate(m_Characters[ID], transform.position, transform.rotation);
		m_HandTransform = m_CurrentChar.transform.Find("BallAttachment");
		m_HeadTransform = m_CurrentChar.transform.Find("StunAttachment");
		m_StunParticle = (GameObject)Instantiate(m_StunParticle,  transform.position, transform.rotation);
		m_CurrentCharacter = ID;
	}
	
		
	void SetColour(Vector4 colour)
	{
		//m_CurrentChar.renderer.material.SetVector("_Color", colour);
	}
	
	public GameObject GetCharacterGameObject()
	{
		return m_CurrentChar;
	}
	
	public int GetCharacter()
	{
		return m_CurrentCharacter;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Globals.g_GamePaused || Globals.g_GameHasEnded)
			return;
		
		if(b_HoldingBall)
		{
			//print (m_HandTransform.position);
			
			Vector3 offset = new Vector3(0.0f,0.0f,0.0f);
			m_HeldBall.transform.position = m_HandTransform.gameObject.GetComponent<SkinnedMeshRenderer>().rootBone.position + offset;

			aimObject.SetActive(true);

			float distFromChar = 40 + ((m_MaxChargeTime-m_ChargeShootTimer)*100.0f);




			aimPos = transform.position;

			if(m_OtherPlayer == 0)
				aimPos.z -= distFromChar; //+ for bottom player
			else
				aimPos.z += distFromChar;

			aimPos.y += 5;

			aimPos.x += m_Velocity.x* m_ShootDeviation * distFromChar*0.7f; //same as z diff

			aimObject.transform.position = aimPos;

			//print (m_ShootDeviation);

		}
		else
		{
			aimObject.SetActive(false);

		}

		SetScoreFire();

		if(m_EndGame == false)
			m_Paused = IsInputCanceled();
		else
			m_Paused = true;
		
		
		
		if(m_StunTime > 0)
		{
			m_StunParticle.SetActive(true);
			
			m_StunParticle.transform.position = m_HeadTransform.gameObject.GetComponent<SkinnedMeshRenderer>().rootBone.position;
			
			m_StunParticle.transform.rotation = m_HeadTransform.gameObject.GetComponent<SkinnedMeshRenderer>().rootBone.rotation;
			
			SimplePropAnimation pAnim = m_StunParticle.GetComponent<SimplePropAnimation>();
			pAnim.Play ("Neutral",1.0f,true);
			
			pAnim.m_Pos = m_StunParticle.transform.position;
			
			
		}
		else
		{
			m_StunParticle.SetActive(false);
			
		}


		//shoot animation
		if(m_ShootAnimTimer > 0.0f)
		{
			m_ShootAnimTimer -= Time.deltaTime;
		}else
		{
			m_ShootAnimTimer = 0.0f;
		}			
		
		//Do movement here
		if(m_StunTime <= 0)
		{
			
			float input = 0;
			if(!m_Paused)
				input = Input.GetAxis("Horizontal" + m_PlayerController.ToString());
			if (input <= m_DeadZone && input >= -m_DeadZone)
				input = 0;
			
			float velocityX = input * playerSpeed * Time.deltaTime;
		
			m_Velocity.x = velocityX;
			Vector3 newPos = this.transform.position + new Vector3(velocityX, 0, 0);
		
			//potential bug, can't shoot while at boundary, test later
			//check left boundary
			if (newPos.x < -xLimit)
				newPos = new Vector3(Globals.g_FieldPoints[Globals.g_LevelIDStatic][0][0]*Globals.g_FieldWidth+1,this.transform.position.y,this.transform.position.z);
			//check right boundary
			if (newPos.x > xLimit)
				newPos = new Vector3(Globals.g_FieldPoints[Globals.g_LevelIDStatic][0][0]*-Globals.g_FieldWidth-1,this.transform.position.y,this.transform.position.z);
				
			SimplePropAnimation t_Animation = m_CurrentChar.GetComponent<SimplePropAnimation>();
			this.transform.position = newPos;
			t_Animation.m_Pos = newPos;
			
			if(m_ShootAnimTimer > 0.0f)
			{
				t_Animation.Play("Throw",m_ShootAnimSpeed);	
			}else
			if(m_Velocity.magnitude<0.3)
			{
				if (Globals.g_Winner == -1)
				if(m_CurrentCharacter !=1)
					t_Animation.Play("Neutral");
				else
				{
					if(b_HoldingBall)
						t_Animation.Play("NeutralBall");
					else
						t_Animation.Play("Neutral");

				}
			}
			else
			{
				t_Animation.m_Orientation = transform.rotation;
				if((m_Velocity.x > 0.0f && m_PlayerController == 0) || (m_Velocity.x<0 && m_PlayerController==1))
				{
					if(b_HoldingBall)
					{
						t_Animation.Play("BallWalkRight", 2.0f);
						
					}else
					{
						t_Animation.Play("WalkRight", 2.0f);
					}
				}else
				{
					if(b_HoldingBall)
					{
						//Stupid fix for the animation problem
						if(m_CurrentCharacter == 1)
							t_Animation.Play("BallWalkRight", 2.0f);
						else
							t_Animation.Play("BallWalkLeft", 2.0f);
						
					}else
					{
						t_Animation.Play("WalkLeft", 2.0f);
					}
				}
				/*t_Animation.Play("Walk", 2.0f, !((m_Velocity.x>0 && m_PlayerController==0) ||
												(m_Velocity.x<0 && m_PlayerController==1))?(false):(true));
											*/				
			}
		
		}else
		{
			//stunned
			SimplePropAnimation t_Animation = m_CurrentChar.GetComponent<SimplePropAnimation>();
			this.transform.position = this.transform.position;		
			t_Animation.Play("Stun", 1.0f, false);
		}
		
		Shoot ();
		
			
		
		
		if(m_StunTime > 0.0f)
		{
			
			//renderer.material.color = new Color(0.2f,0.2f,1.0f,1.0f);
			//m_CurrentChar.renderer.material.color = new Color(0.2f,0.2f,1.0f,1.0f);
			SetColour(new Vector4(0.2f,0.2f,1.0f,1.0f));
		}
		else
		{
			
			//renderer.material.color = m_MyColor;
			//m_CurrentChar.renderer.material.color = m_MyColor;
			SetColour(new Vector4(m_MyColor.r,m_MyColor.g,m_MyColor.b,m_MyColor.a));
		}
		
		

	}
	
	private void SetScoreFire()
	{
		int Score = Globals.g_ScoreController.GetScore(m_PlayerController);
		
		if(m_PlayerController == 1)
		{
			if(Score == 0)
			{
				for(int i = 0; i < 3; i++)
				{
					m_FireBlueList[i].SetActive(false);
				}
			}
			else
			if(Score == 1)
			{
				m_FireBlueList[0].SetActive(true);
			}
			else
			if(Score == 2)
			{
				m_FireBlueList[1].SetActive(true);
			}
			else
			if(Score == 3)
			{
				m_FireBlueList[2].SetActive(true);
			}
		}
		else
		if(m_PlayerController == 0)
		{
			if(Score == 0)
			{
				for(int i = 0; i < 3; i++)
				{
					m_FireRedList[i].SetActive(false);
				}
			}
			else
			if(Score == 1)
			{
				m_FireRedList[0].SetActive(true);
			}
			else
			if(Score == 2)
			{
				m_FireRedList[1].SetActive(true);
			}
			else
			if(Score == 3)
			{
				m_FireRedList[2].SetActive(true);
			}
		}
		
	}
	
	public Vector3 GetVelocity()
	{
		return m_Velocity;
	}
	
	[RPC]
	public void SendVel(Vector3 vel)
	{
		m_Velocity = vel;
	}
	
	[RPC]
	void SendInput(float HInput, float SInput)
	{
		m_CurrentServerHInput = HInput;
		m_CurrentServerSInput = SInput;
	}
	
	[RPC]
	public void SendServerH(float h)
	{
		m_ServerH = h;
	}
	
	[RPC]
	void SendServerStunTime(float time)
	{
		Globals.g_Players[0].GetComponent<Player>().SetStunTime(time);
		m_ServerStunTime += time;
	}
	
	[RPC]
	void SendClientStunTime(float time)
	{
		Globals.g_Players[1].GetComponent<Player>().SetStunTime(time);
	}
	
	public void SpawnGuards()
	{
		DestroyGuards();
		foreach (GameObject guard in m_Guards)
		{
			if (guard.GetComponent<PowerUpGuard>().getOwnerID() == GetID())
			{

				GameObject.Instantiate(guard);
			}
		}
	}
	
	public void DestroyGuards()
	{
		GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");
		for (int i = 0; i < guards.Length; i++)
		{
			if (guards[i].GetComponent<PowerUpGuard>().getOwnerID() == m_PlayerController)
			{

				Destroy(guards[i]);
			}
		}
	}
	
	public int GetPlayerController()
	{
		return m_PlayerController;
	}
	
	public Ball GetHeldBall()
	{
		if(!b_HoldingBall) return null;
		return m_HeldBall;
	}
	
	void Shoot()
	{

		if(!m_Paused)
			m_CurrentServerSInput = Input.GetAxis("Shoot"+m_PlayerController.ToString());
		
		if (m_CurrentServerSInput <= m_DeadZone && m_CurrentServerSInput >= -m_DeadZone)
			m_CurrentServerSInput = 0;

		
		Vector3 shootVelocity;
		
		if (m_StunTime <= 0)
		{
			
			m_StunTime = 0;
			//renderer.material.color = m_MyColor;
			SetColour(new Vector4(m_MyColor.r,m_MyColor.g,m_MyColor.b,m_MyColor.a));
			//shooting
			//shooting
			if(b_Charged || m_ShootTimer <= 0)
			{
				if(b_Charged || m_CurrentServerSInput >= m_DeadZone) //if local = if(b_Charged || Input.GetAxis("Shoot"+m_PlayerController.ToString()) == 1.0f)
				{
					if(b_Charged || b_HoldingBall && b_SameButtonShootBuffer)
					{		
						if(!b_Charged)// && m_HeldBall.GetComponent<Ball>().GetBallType() == Ball.BallTypes.NORMAL)
						{
							if(m_HeldBall.GetComponent<Ball>().GetBallType() == Ball.BallTypes.NORMAL)
							{
								if(!b_Charging) //start charging
								{
									m_ChargeShootTimer = m_MaxChargeTime;//Globals.g_MaxChargeTime;							
									b_Charging = true;	
									m_HeldBall.renderer.material.SetFloat("_Show", 1.0f);
	                                m_HeldBall.renderer.material.SetFloat("_Size", 0.0f);
									//print("charging");
								}
								else
								{
									m_ChargeShootTimer -= Time.deltaTime;
									if(m_ChargeShootTimer<=0) m_ChargeShootTimer = 0;
									
									m_HeldBall.renderer.material.SetFloat("_Show", 1.0f);
									m_HeldBall.renderer.material.SetFloat("_Size", ((m_MaxChargeTime-m_ChargeShootTimer)/m_MaxChargeTime)-m_MaxChargeTime);///10.0f);
									
									if(m_ChargeShootTimer <= 0.0f)
									{
										//b_Charged = true;
										//b_Charging = false;
										//print ("charged");
									}
								}
							}else//special balls are always at max charge
							{
								print("specialCharge");
								m_ChargeShootTimer = 0.0f;
								b_Charged = true;
								b_Charging = false;								
								
								m_HeldBall.renderer.material.SetFloat("_Show", 1.0f);
								m_HeldBall.renderer.material.SetFloat("_Size", 1.0f);								
							}
							
						}
						else //check if ball just changed to special
						{
							//shoot animation
							SimplePropAnimation t_Animation = m_CurrentChar.GetComponent<SimplePropAnimation>();
							this.transform.position = this.transform.position;		
							t_Animation.Play("Throw",m_ShootAnimSpeed);		
							m_ShootAnimTimer = m_ShootAnimationLengthInSeconds;
							//print ("SHOOT");
							//print(b_HoldingBall);
							//print(b_SameButtonShootBuffer);
									
							shootVelocity = Random.onUnitSphere;
							shootVelocity.y = 0.0f;
							shootVelocity.z = 1.0f;//Random.Range(0.5f,2f);
							shootVelocity.x = 0.0f;
							
							//deviate shot in x by Globals.g_AimDeviationAngle
							/*
							if(Mathf.Abs(Input.GetAxis("Horizontal"+m_PlayerController.ToString())) >= 0.2f)
							{
								float multiplier = Input.GetAxis("Horizontal" + m_PlayerController.ToString()) < 0?-1:1;
								float angleInRadians = (Globals.g_AimDeviationAngle/180)*Mathf.PI;
								
								float sin = Mathf.Sin(angleInRadians);
								float cos = Mathf.Cos(angleInRadians);
								
								shootVelocity.x = sin*multiplier;
								shootVelocity.z = cos;
							}
							*/
							shootVelocity.x = m_Velocity.x* m_ShootDeviation;
							
							float chargeShotMultiplier = Mathf.Lerp(1.0f,0.3f,m_ChargeShootTimer/m_MaxChargeTime);
						
							
							shootVelocity *= ((Globals.g_BallSpeed*chargeShotMultiplier)/shootVelocity.magnitude);
							//print (shootVelocity);
							//print (chargeShotMultiplier);
							b_Charged = false;
							b_Charging = false;
							
							if(m_PlayerController == 1)
							{
								shootVelocity.z *= -1;
							}	
						
							if(m_PowerShot == true)
							{					
								m_PowerShot = false;
							}
							
							foreach(GameObject t_Ball in Globals.g_Balls)
								if(t_Ball.GetComponent<Ball>().GetSide()==m_MySide || t_Ball.GetComponent<Ball>().GetSide()==Ball.Sides.NOSIDE)
									t_Ball.renderer.material.SetFloat("_Show", 0.0f);
							
							//prevent self shoot
							float offset = 0.0f;
							
							if(this.GetID() == 0)
							{
								
								offset = 17.0f;
							}
							else
							{
								offset= -17.0f;
								
							}
							
							m_HeldBall.transform.position = this.transform.position + new Vector3(0,0,offset);
							m_HeldBall.ShootBall(shootVelocity);

							m_HeldBall.PlayThrowSound(transform.position);
							
							m_ShootTimer += m_ShootDelay;
							b_HoldingBall = false;
							b_SameButtonShootBuffer = false;
						}
						
					}else if(!b_HoldingBall && b_SameButtonShootBuffer)
					{
						//Hold ball
						Ball closestBall = GetClosestInactiveBall();
						if(closestBall != null)
						{
							foreach(GameObject t_Ball in Globals.g_Balls)
								if(t_Ball.GetComponent<Ball>().GetSide()==m_MySide || t_Ball.GetComponent<Ball>().GetSide()==Ball.Sides.NOSIDE)
									t_Ball.renderer.material.SetFloat("_Show", 0.0f);
							if(m_MaxShootDistance > (closestBall.transform.position-transform.position).magnitude)
							{
								closestBall.PlayPickUpSound(closestBall.transform.position);
								closestBall.SetHeld(this);
								m_HeldBall = closestBall;
								m_HeldBall.renderer.material.SetFloat ("_Show", 1.0f);
								b_SameButtonShootBuffer = false;
								b_HoldingBall = true;
							}
						}
						
					}
				}
				else if(m_CurrentServerSInput == 0.0f) //If local = Input.GetAxis("Shoot"+m_PlayerController.ToString()) == 0.0f
				{
					b_SameButtonShootBuffer = true;
					if(b_Charging)
					{
						b_Charged = true;
					}
				}
			}else
			{
				m_ShootTimer -= Time.deltaTime;
			}
			
		}else
		{

			m_StunTime -= Time.deltaTime;				

			//renderer.material.color = new Color(0.2f,0.2f,1.0f,1.0f);
			//SetColour(new Vector4(0.2f,0.2f,1.0f,1.0f));
			//print (m_StunTime);
		}
		
		//check if holding a ball to apply special to it
		if(b_SpecialAvailable && b_HoldingBall)
		{
			b_SpecialAvailable = false;
			
			if(m_Special == -1)
			{
				m_HeldBall.SetRandomType();//m_HeldBall.SetType(m_Special);
			}else // m_Special == ballType
			{
				//set to specific type
				ChangeBallTypesAtMySide(0);
				m_HeldBall.SetType(m_Special);
				//reset holding state
				b_Charged = false;
				b_Charging = false;
				b_SameButtonShootBuffer = false;
				m_ChargeShootTimer = 0.0f;
			}
		}		
		
		// Code for showing which ball we're going to pick up.
		Ball t_ClosestBall = GetClosestInactiveBall();
		
		foreach(GameObject t_Ball in Globals.g_Balls)
			if(t_Ball.GetComponent<Ball>().GetSide()==m_MySide || t_Ball.GetComponent<Ball>().GetSide()==Ball.Sides.NOSIDE)
				t_Ball.renderer.material.SetFloat("_Show", 0.0f);
		
		if(b_HoldingBall) m_HeldBall.renderer.material.SetFloat ("_Show", 1.0f);
		if(t_ClosestBall != null && b_HoldingBall == false && m_MaxShootDistance > (t_ClosestBall.transform.position-transform.position).magnitude)
		{
			t_ClosestBall.gameObject.renderer.material.SetFloat("_Show", 1.0f);
			t_ClosestBall.gameObject.renderer.material.SetFloat("_Size", 0.1f);
		}
		
		//Check if game is over
		//CheckEndTime (); Do this if local
		if(Globals.g_IsNetworkStatic == false)
		{
			CheckEndTime();
		}
		
	
	}
	
	[RPC]
	void PickUpBall(Ball heldBall,bool sameShoot ,bool holdingBall)
	{
		m_HeldBall = heldBall;
		b_SameButtonShootBuffer = sameShoot;
		b_HoldingBall = holdingBall;
		
	}

	
	public void ChangeBallTypesAtMySide(int a_type)
	{
		foreach (GameObject ball in Globals.g_Balls)
		{
			Ball t_ball = ball.GetComponent<Ball>();
			if (t_ball.GetSide() != m_MySide)
				continue;
			
			t_ball.SetType(a_type);
		}
	}
	
	public void GiveSpecialToken(int a_Token = -1)
	{
		b_SpecialAvailable = true;
		m_Special = a_Token;
		if (!b_HoldingBall)
			ChangeBallTypesAtMySide(a_Token);
	}
	
	public int GetSpecialToken()
	{
		if (b_SpecialAvailable || (GetHeldBall() != null && GetHeldBall().GetBallType() != Ball.BallTypes.NORMAL))
			return m_Special;
		return -1;
	}
	
	public void ResetPlayer()
	{
		DestroyGuards();
		b_SameButtonShootBuffer = false;
		b_HoldingBall = false;
		b_Charged = false;
		m_ChargeShootTimer = 0.0f;
		b_Charging = false;
		transform.position = m_InitialPosition;
		m_HeldBall = null;
		m_StunTime = 0.0f;
		b_SpecialAvailable = false;
		ChangeBallTypesAtMySide(0);
		m_EndedTheGame = true;
	}
	
	void CheckEndTime()
	{
		/*if(Globals.g_Timer.GetTime() < 0)
		{
			
			for(int i = 0; i < 2; i++)
			{
				if(Globals.g_Players[i].GetComponent<Player>().GetBallsOnSide() > Globals.g_TotalBalls/2)
				{
					Globals.g_Players[i].GetComponent<Player>().EndGame(false);
					break;
				}
			}
			
			EndGame (true);
		}*/
		
	}
	
	public void EndGame(bool end/*bool a_Draw*/)
	{
		/*if(!a_Draw)
			Globals.g_ScoreController.IncreaseForPlayer(m_OtherPlayer);

		for(int i = 0; i < 2; i++)
			Globals.g_Players[i].GetComponent<Player>().ResetPlayer();
		
		//Reset everything
		for(int i = 0; i < m_TotalBalls; i++)
		{
			m_BallObjects[i].GetComponent<Ball>().RestartBall();
		}
		
		
		
		Globals.g_Timer.ResetTime();*/
		m_EndGame = end;
		
	}
	
	public void CountBalls()
	{
		int numberOfBalls = 0;
		for(int i = 0; i < m_TotalBalls; i++)
		{
			if(Globals.g_Balls[i].GetComponent<Ball>().GetSide() == m_MySide)
			{
				numberOfBalls++;
			}
			
		}
		
		m_BallsOnSide = numberOfBalls;

	}
	
	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Ball")
		{
			col.gameObject.GetComponent<Ball>().StopBall();
		}
	}
	
	
	
	public int GetControllerID()
	{
		return m_PlayerController;	
	}
	
	Ball GetClosestInactiveBall()
	{
		GameObject[] Balls = GameObject.FindGameObjectsWithTag("Ball");
		float distanceSqr = float.MaxValue;
		float ballDistSqr;
		Ball closestBall = null;
		
		for(int i = 0; i < Balls.Length; i++)
		{
			//check only inactive balls
			if(Balls[i].GetComponent<Ball>().IsActive())
				continue;
			
			ballDistSqr = (Balls[i].transform.position-transform.position).sqrMagnitude;
			if(distanceSqr > ballDistSqr)
			{
				distanceSqr = ballDistSqr;
				closestBall = Balls[i].GetComponent<Ball>();
			}
		}
		
		return closestBall;
	}
	
	public void AddStun(float seconds)
	{
		m_StunTime += seconds;
	}
	
	public int GetBallsOnSide()
	{
		return m_BallsOnSide;
	}
	
	public int GetID()
	{
		return m_PlayerController;
	}
	
	public void SetID(int ID)
	{
		m_PlayerController = ID;
	}
	
	public NetworkPlayer GetOwner()
	{
		return m_Owner;
	}
	
	public int GetOpponent()
	{
		return m_OtherPlayer;
	}
	
	public void SetOwner(NetworkPlayer owner)
	{
		m_Owner = owner;
	}
	
	public Ball.Sides GetSide()
	{
		return m_MySide;
		
	}
}
