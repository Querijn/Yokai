using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour 
{
	public enum Sides
	{
		NOSIDE = -1,
		TOP,
		BOTTOM
		
	};
	
	public enum BallTypes
	{
		NORMAL = 0,
		LIGHT,
		EXPLOSIVE,
		//POWER,
		CURVE,
		STICKY,
		ROCKET,
		HEAVY,
		MAX
	};
	
	public BallTypes[] m_StunPowerups;
	
	public static float m_CurveMagnitude = 500.0f;
	
	public enum CollisionPriorities
	{
		LOWEST = 0,
		LOW,
		MIDDLE,
		HIGH,
		HIGHEST	
	};
	
	public bool m_Stun;
	
	private BallTypes m_BallType = BallTypes.NORMAL;
	private float m_StunTime = Globals.g_BallStunDefault;
	private Vector3 m_Velocity = new Vector3(0,0,0);
	private Vector3 m_InitialPosition = new Vector3(0,0,0);
	private Vector3 m_CurveVector = new Vector3(0,0,0);
	
	//Array of all the balls
	private GameObject[] m_BallObjects;

	//Ball stuff
	private CollisionPriorities m_CollisionPriority = CollisionPriorities.LOW;
	private float m_Radius = 0;
	private bool b_Active = true;
	public bool b_IgnoreBump = true;
	private Sides m_PlayerSide = Sides.NOSIDE;
	private bool b_Held = false;
	private Player m_HoldingPlayer = null;
	private Player m_ParentPlayer = null;
	private bool b_SpecialActionDone = false;
	private bool m_Collided = false;
	private float m_MoveParticleInterval = 0.0f;
	private ParticleSystem m_Gravel;
	
	public ParticleSystem g_SlimeParticle;
	public ParticleSystem g_RocketParticle;
	
	public enum SoundEffects
	{
		EXPLODE1 = 0,
		EXPLODE2,
		HEAVYBALL1,
		ROCKET1,
		ROCKET2,
		ROCKET3,
		ROCKET4,
		ROCKETLOOP,
		BALLCOL1,
		BALLCOL2,
		BALLCOL3,
		BALLCOL4,
		BALLCOL5,
		BALLCOL6,
		BALLCOL7,
		BALLROLL,
		STICKYCOL1,
		STICKYCOL2,
		STICKYCOL3,
		STICKYCOL4,
		STICKYCOL5,
		STICKYROLL1,
		STICKYROLL2,
		PICKUP1,
		PICKUP2,
		PICKUP3,
		POWERUP,
		PINBALL1,
		PINBALL2,
		PINBALL3,
		BADGERHIT1,
		BADGERHIT2,
		CHICKENHIT1,
		CHICKENHIT2,
		MISS1,
		MISS2,
		MISS3,
		MISS4,
		MISS5,
		MISS6,
		MISS7,
		MISS8,
		MISS9,
		MISS10,
		MAX
		
		
	};
	
	public AudioClip[] AudioClips;// = new AudioSource[SoundEffects.MAX];
	
	public AudioSource Source;
	public AudioSource RollSource;
	//public AudioSource m_AudioSouce;
	
	// Use this for initialization
	void Start() 
	{
		m_Radius =Globals.g_BallRadius;// transform.localScale.x/2;
		//initial random velocty - DEBUG
		//m_Velocity = Random.onUnitSphere;
		//m_Velocity.y = 0.0f;
		//m_Velocity *= (Globals.g_BallSpeed/m_Velocity.magnitude); //normalises ball velocity to m_BallSpeed after y component removal
		
		m_BallObjects = Globals.g_Balls;
		
		m_InitialPosition = this.transform.position;
		
		
		m_Collided = false;
		
		m_CurveVector = new Vector3(0,0,0);
		
		//renderer.material.color = new Color(0.0f,1.0f,0.0f,1f); //normal ball
		renderer.material = Globals.g_BallMaterials[(int)BallTypes.NORMAL];
		
		//m_RocketTrail = (ParticleSystem)Instantiate(m_RocketTrail,  transform.position, transform.rotation);
		
		//m_RocketTrail.Stop(true);
		
		g_SlimeParticle = (ParticleSystem)Instantiate(g_SlimeParticle);
		//g_SlimeParticle.transform.Rotate(new Vector3(0,0,1),-90);
		
		g_SlimeParticle.Stop();
		
		g_RocketParticle = (ParticleSystem)Instantiate(g_RocketParticle);
		
		g_RocketParticle.Stop ();
	}
	
	public void PlayExplosionSound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay =Random.Range((int)SoundEffects.EXPLODE1, (int)SoundEffects.EXPLODE2);
		Source.clip = AudioClips[soundToPlay];
		Source.Play();
	}
	
	public void PlayRocketExplosionSound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay =(int)SoundEffects.ROCKETLOOP;
		Source.clip = AudioClips[soundToPlay];
		Source.Play();	
	}
	
	public void PlayRocketLoopSound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay =Random.Range((int)SoundEffects.ROCKET1, (int)SoundEffects.ROCKET4);
		Source.clip = AudioClips[soundToPlay];
		Source.Play();	
	}

	public void PlayPlayerHitSound(Vector3 position, int character)
	{	
		Source.transform.position = transform.position;

		int soundToPlay = (int)SoundEffects.BADGERHIT1;

		if(character == 0)
		{
			soundToPlay = Random.Range((int)SoundEffects.BADGERHIT1, (int)SoundEffects.BADGERHIT2);
		}
		else
		{
			soundToPlay = Random.Range((int)SoundEffects.CHICKENHIT1, (int)SoundEffects.CHICKENHIT2);
		}


		Source.clip = AudioClips[soundToPlay];
		Source.Play();	
	}

	public void PlayPickUpSound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay =Random.Range((int)SoundEffects.PICKUP1, (int)SoundEffects.PICKUP3);
		Source.clip = AudioClips[soundToPlay];
		Source.Play();
	}

	public void PlayPowerUpSound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay =(int)SoundEffects.POWERUP;
		Source.clip = AudioClips[soundToPlay];
		Source.Play();
	}

	public void PlayPinballSound(Vector3 position) //todo
	{	
		Source.transform.position = transform.position;
		int soundToPlay =Random.Range((int)SoundEffects.PINBALL1, (int)SoundEffects.PINBALL3);
		Source.clip = AudioClips[soundToPlay];
		Source.Play();
	}

	public void PlayThrowSound(Vector3 position) //todo
	{	
		Source.transform.position = transform.position;
		int soundToPlay = Random.Range((int)SoundEffects.MISS1, (int)SoundEffects.MISS10);
		Source.clip = AudioClips[soundToPlay];
		Source.Play();
	}
	
	public void PlayBallRollSound(Vector3 position)
	{	
		RollSource.transform.position = transform.position;
		int soundToPlay = (int)SoundEffects.BALLROLL;
		RollSource.clip = AudioClips[soundToPlay];
		RollSource.Play();	
	}
	
	public void StopRollSounds()
	{	
		RollSource.Stop();
	}
	
	public void PlayStickyRollSound(Vector3 position)
	{	
		RollSource.transform.position = transform.position;
		int soundToPlay = Random.Range((int)SoundEffects.STICKYROLL1, (int)SoundEffects.STICKYROLL2);
		RollSource.clip = AudioClips[soundToPlay];
		RollSource.Play();	
	}
	
	
	public void PlayBallCollisionSound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay =Random.Range((int)SoundEffects.BALLCOL1, (int)SoundEffects.BALLCOL7);
		Source.clip = AudioClips[soundToPlay];
		Source.Play();	
	}
	
	public void PlayStickyCollisionSound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay =Random.Range((int)SoundEffects.STICKYCOL1, (int)SoundEffects.STICKYCOL5);
		Source.clip = AudioClips[soundToPlay];
		Source.Play();	
	}
	
	public void PlayHeavySound(Vector3 position)
	{	
		Source.transform.position = transform.position;
		int soundToPlay = (int)SoundEffects.HEAVYBALL1;
		Source.clip = AudioClips[soundToPlay];
		Source.Play();	
	}
	
	public void StopBall()
	{
		m_Velocity = new Vector3(0,0,0);
		StopRollSounds();
		
	}
	
	public void PlayStickyParticle()
	{
		
		
	}
	
	public bool GetHeld()
	{
		return b_Held;
		
	}
	
	public void ShootBall(Vector3 direction, float offset = 3.0f)
	{
		TrailRenderer tRender = GetComponent<TrailRenderer>();
		tRender.enabled = true;		
		
		b_Held = false;
		b_Active = true;
		m_Velocity = direction;
		m_PlayerSide = Sides.NOSIDE; 
		transform.position += m_Velocity.normalized*offset; //move ball a little to prevent collision to player
		
		if(GetBallType() == BallTypes.STICKY)
		{
			g_SlimeParticle.Play();
			PlayStickyRollSound(transform.position);
		}
			else
		PlayBallRollSound(transform.position);
		
		if(GetBallType() == BallTypes.ROCKET)
		{	
			g_RocketParticle.transform.position = new Vector3(0,-1000,0);
			g_RocketParticle.Play();
		}
	}

	
	public float GetRadius()
	{
		return m_Radius;
		
	}
	
	public Player GetParentPlayer()
	{
		return m_ParentPlayer;
		
	}
	
	public void SetParentPlayer(Player player)
	{
		m_ParentPlayer = player;
		
	}
	
	void SetBallZ(float a_Value)
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, a_Value);
	}
	
	void SetBallX(float a_Value)
	{
		transform.position = new Vector3(a_Value, transform.position.y, transform.position.z);
	}
	
	public Vector3 GetVelocity()
	{
		return m_Velocity;
	}
	
	public void SetVelocity(Vector3 newVel)
	{
		m_Velocity = newVel;
	}
	
	public Color GetBallTypeColour(BallTypes a_BallType)
	{
		//we cannot send a RPC using a color, so we use vector3 instead
		Vector3 setColor = new Vector3(1,1,1);
		
		//if (b_Active)
		{
			switch(a_BallType) //COLOR
			{
			case BallTypes.NORMAL:
				setColor = new Vector3(1.0f,1.0f,1.0f);
				break;
			case BallTypes.CURVE:
				setColor = new Vector3(0.8f,0.2f,0.7f);
				break;
			case BallTypes.ROCKET:
				setColor = new Vector3(0.8f,0.4f,0.2f);
				break;
			case BallTypes.HEAVY:
				setColor = new Vector3(0.6f,0.1f,0.2f);
				break;
			case BallTypes.LIGHT:
				setColor = new Vector3(0.8f,0.7f,0.15f);
				break;
			case BallTypes.STICKY:
				setColor = new Vector3(0.2f,0.85f,0.2f);
				break;
			case BallTypes.EXPLOSIVE:
				setColor = new Vector3(0.8f,0.15f,0.15f);
				break;
			}
		}
		//else
		//	setColor = new Vector3(0.0f,1.0f,0.0f);
		
		print (setColor);
		
		return new Color(setColor.x,setColor.y,setColor.z);
	}
	
	//we cannot send a RPC using a color, so we use vector3 instead
	[RPC]
	public void SendColor(Vector3 color)
	{
		renderer.material.color = new Color(color.x,color.y,color.z,1.0f);
	}
	
	void UpdateSpecial()
	{	

		if(!b_Held && !b_SpecialActionDone) //BEHAVIOUR
		{	
			
			switch(m_BallType)
			{
			case BallTypes.CURVE:
				float midSection = Globals.g_FieldHeight/2.0f;
				if(transform.position.z < (Globals.g_PlayerInitialPos[0].z+Globals.g_FieldHeight)+midSection &&
					transform.position.z > (Globals.g_PlayerInitialPos[0].z+Globals.g_FieldHeight)-midSection)
				{				
					if(m_CurveVector.x == 0.0f)
					{
						m_CurveVector.x = -m_Velocity.x;
						//m_CurveVector.x *= -1.0f;
						m_CurveVector.Normalize();
						//print ("curve");
						//print (m_CurveVector);						
						
					}
					//print ("Action done");					
					
					//m_Velocity.x *= -1;
					//b_SpecialActionDone = true; //keep false
				}
				m_Velocity += m_CurveVector*Time.deltaTime*m_CurveMagnitude;
				if(m_Velocity.magnitude !=0)
				m_Velocity *= (Globals.g_BallSpeed/m_Velocity.magnitude); // ensure max speed isn't crossed
				
				break;
/*			case BallTypes.POWER:
				m_Velocity *= 2;
				b_SpecialActionDone = true;
				break;*/
			case BallTypes.EXPLOSIVE:
				if(m_Collided == true)
				{
					for(int i = 0; i < Globals.g_Balls.Length; i++)
					{
						Ball otherBall = Globals.g_Balls[i].GetComponent<Ball>();
						if(otherBall != this)
						if(Globals.SphereCollision(transform.position, m_Radius, otherBall.transform.position, Globals.g_ExplosionRadius))
						{
							Vector3 diff = otherBall.transform.position - transform.position;
							
							float power = 1.0f + (diff.magnitude/Globals.g_FieldHeight);
							
							Vector3 newVel = diff.normalized * Globals.g_BallSpeed * power; //Change 1.5f to power for bomb power depending on how far the ball was
							
							otherBall.SetVelocity(newVel);
							
							PlayExplosionSound(transform.position);
							
						//	m_Sounds.Sounds[0
							
							//print ("BOOM");
						}
					}

					if(Globals.g_Obstacles != null && Globals.g_CurrentGameModeStatic == Globals.GameModes.TUGOFWAR)
					for(int i = 0; i < Globals.g_Obstacles.Length; i++)
					{
						Obstacle ob = Globals.g_Obstacles[i].GetComponent<Obstacle>();
						if(Globals.SphereCollision(transform.position, m_Radius, ob.transform.position, Globals.g_ExplosionRadius))
						{
							Vector3 diff = ob.transform.position - transform.position;
							
							float power = 1.0f + (diff.magnitude/Globals.g_FieldHeight);
							
							Vector3 newVel = diff.normalized * 700 * power; //Change 1.5f to power for bomb power depending on how far the ball was
							
							ob.SetVelocity(newVel);
							
							PlayExplosionSound(transform.position);
							
							//	m_Sounds.Sounds[0
							
							//print ("BOOM");
						}
					}


					//BLOW UP
					PlayExplosionSound(transform.position);
					b_SpecialActionDone = true;
					PlayExplosionParticle();
				}
							
				break;
			case BallTypes.ROCKET:
				if(m_Collided == true)
				{
					Player targetPlayer, player1, player2;
					player1 = Globals.g_Players[0].GetComponent<Player>();
					player2 = Globals.g_Players[1].GetComponent<Player>();
					
					if(player1 == m_ParentPlayer)
					{
						targetPlayer = player2;
					}
					else
					{
						targetPlayer = player1;
					}
				
					Vector3 diff = targetPlayer.transform.position - this.transform.position;
					
					m_Velocity = diff.normalized * Globals.g_BallSpeed * Globals.g_RocketSpeed;
					
					b_SpecialActionDone = true;//Uncomment to disable "homing effect"
					
					PlayRocketExplosionSound(transform.position);
					PlayRocketLoopSound(transform.position);
				}
							
				break;
			case BallTypes.HEAVY:
				m_Velocity /= 2.0f;
				m_StunTime = Globals.g_BallStunDefault*2.0f;
				PlayHeavySound(transform.position);
				b_SpecialActionDone = true;
				break;
			case BallTypes.LIGHT:
				m_Velocity *= 3;
				m_StunTime = 0.0f;
				b_SpecialActionDone = true;
				break;
			case BallTypes.STICKY:
				g_SlimeParticle.transform.position = transform.position;
				Vector3 Dir = -m_Velocity.normalized;
				g_SlimeParticle.transform.rotation = Quaternion.Euler(new Vector3(0,Mathf.Atan2(Dir.x,Dir.z)*Mathf.Rad2Deg,0));//new Quaternion(0,Mathf.Atan2(Dir.x,Dir.z),0, 1);
				g_SlimeParticle.transform.Rotate(new Vector3(0,0,1), -90.0f);
				break;
			default:
				break;
			};
		}
	}
	
	void ApplyFakeFieldBump(float magnitude)
	{
		if(b_IgnoreBump) return;
		
		float factor = (transform.position.z/Globals.g_FieldHeight); // [-1,1]
		float normalisedDistFromMid = 1.0f-Mathf.Abs(factor);
		if(normalisedDistFromMid > 0.5f) // distance from mid
		{
			float finalFactor = (normalisedDistFromMid-0.5f)/0.5f;//[0,1]
			float direction = (factor<0)?-1.0f:1.0f;
			
			Vector3 addedVector = new Vector3(0,0,direction*magnitude*finalFactor);
			//print (addedVector);
			this.m_Velocity += addedVector;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if(Globals.g_GamePaused)
			return;


		renderer.material = Globals.g_BallMaterials[(int)m_BallType];

		//m_Trail.enabled = true;	
		//change color based on type
		//renderer.material.color = new Color(0.0f,1.0f,0.0f,1f); //normal ball
		
		if(m_BallType != BallTypes.NORMAL)
		{
			UpdateSpecial();
		}
		
		if(m_BallType == BallTypes.ROCKET)
		{
			if(b_SpecialActionDone == true)
			{
				g_RocketParticle.transform.position = transform.position;
				Vector3 Dir = -m_Velocity.normalized;
				g_RocketParticle.transform.rotation = Quaternion.Euler(new Vector3(0,Mathf.Atan2(Dir.x,Dir.z)*Mathf.Rad2Deg,0));//new Quaternion(0,Mathf.Atan2(Dir.x,Dir.z),0, 1);
				g_RocketParticle.transform.Rotate(new Vector3(0,0,1), -90.0f);
			}
		}
		
		//update position
		//if held
		if(b_Held)
		{
			float offset = 0.0f;
			
			transform.position = m_HoldingPlayer.transform.position; //+offset?
			
			if(m_HoldingPlayer.GetID() == 0)
			{
				
				offset = 15.0f;
			}
			else
			{
				offset= -15.0f;
				
			}
			float tmp = transform.position.z+offset;
			
			transform.position = new Vector3(transform.position.x, transform.position.y,tmp);
			
		}
		else
		{
			if(this.m_PlayerSide==Ball.Sides.NOSIDE)
				PlayGravelParticle();
			else StopGravelParticle();
			
			//find distance moved
			if(m_Velocity.magnitude != 0.0f)
			{
				float distMoved = m_Velocity.magnitude*Time.deltaTime;

				
				Vector3 t_Vel = m_Velocity;
				t_Vel.y = 0;

				transform.position += t_Vel * Time.deltaTime;
				
				
				//rotate
				//get circumference
				float circum = m_Radius*2*Mathf.PI;
				
				float rotationPercentage = (distMoved/circum)*0.5f;
				
				Vector3 rotationAxis = Vector3.Cross(t_Vel.normalized, new Vector3(0,1,0));

				transform.RotateAround(transform.position, rotationAxis,-360.0f*rotationPercentage);

			}
		}
		
		if(Globals.g_CurrentGameModeStatic != Globals.GameModes.CHAOS)
		{
			ApplyFakeFieldBump(Globals.g_FieldBumpModifier);		
		}
		
		//ball collisions check
		HandleBallCollisions();
		
		//Bounds check
		HandleBoundsCollision();
		
		//Player Collision
		HandlePlayerCollision();
	
		FixYVelocity();
		// Fixing the flying balls once and for good.
		
		if (m_PlayerSide != Sides.NOSIDE)
		{
			foreach (GameObject player in Globals.g_Players)
			{
				Player p = player.GetComponent<Player>();
				if (p.GetSide() == m_PlayerSide)
				{
					if (m_BallType != (BallTypes)p.GetSpecialToken())
						if (p.GetHeldBall() == null && p.GetSpecialToken() != -1)
						{
							SetType(p.GetSpecialToken());
						}
				}
			}
		}
	}
	
	[RPC]
	public void SendVel(Vector3 vel)
	{
		m_Velocity = vel;
		
	}
	
	[RPC]
	public void SendType(int type)
	{
		m_BallType = (BallTypes)type;
	}
	
	public void SetCollisionPriority( Ball.BallTypes type)
	{
		//set collision priority
		switch(type)
		{
		case BallTypes.HEAVY:
			m_CollisionPriority = CollisionPriorities.HIGHEST;
			break;
			
		case BallTypes.ROCKET:
			m_CollisionPriority = CollisionPriorities.HIGH;
			break;
			
		case BallTypes.STICKY:
			m_CollisionPriority = CollisionPriorities.MIDDLE;
			break;
			
		case BallTypes.NORMAL:
		case BallTypes.CURVE:
//		case BallTypes.POWER:
			m_CollisionPriority = CollisionPriorities.LOW;
			break;
			
		case BallTypes.LIGHT:
			m_CollisionPriority = CollisionPriorities.LOWEST;
			break;
		default:
			m_CollisionPriority = CollisionPriorities.LOW;
			break;
		}
		
	}
	
	public void SetRandomType(int a_Token = -1)
	{
		//assign random type
		while(m_BallType == BallTypes.NORMAL)
		{
			if(a_Token==-2) 
			{
				int index = Random.Range(0, (int)Globals.GetGlobalObject().g_FieldPowerups.Length);
				m_BallType = Globals.GetGlobalObject().g_FieldPowerups[index];
				print ("Stun "+index);
			}
			else
			{
				int index = Random.Range(0, (int)Globals.GetGlobalObject().g_FieldPowerups.Length);
				m_BallType = Globals.GetGlobalObject().g_FieldPowerups[index];
				print ("No stun "+a_Token);
			} //m_BallType = (BallTypes)Random.Range(0,(int)BallTypes.MAX);
		}
		//m_BallType = BallTypes.CURVE; //DEBUG
		
		SetCollisionPriority(m_BallType);

		
	}
	
	public void SetType(int a_Type)
	{
		m_BallType = (BallTypes)a_Type;
		SetCollisionPriority(m_BallType);
		PlaySpecialParticle(GetBallTypeColour(m_BallType));
	}
	
	
	GameObject[] CheckBallCollisions()
	{
		List<GameObject> t_Collisions = new List<GameObject>();
		
		Transform Ball2;
		
		for(int i = 0; i < Globals.g_Balls.Length; i++)
		{			
			//check current ball against all other balls
			if(gameObject != Globals.g_Balls[i])
			{				
				Ball2 = Globals.g_Balls[i].transform;
				
				if(Ball2.GetComponent<Ball>().IsActive())
				{
					//calculate distance
					Vector3 distanceVector = transform.position - Ball2.position;
					float distance = ((distanceVector).magnitude) - m_Radius*2;
					
					if(distance < 0) // ball radii already taken into account
					{
						b_IgnoreBump = false;
						t_Collisions.Add(Globals.g_Balls[i]);
						
						if((GetBallType() != BallTypes.STICKY && Ball2.GetComponent<Ball>().GetBallType() != BallTypes.STICKY) || (GetBallType() == BallTypes.STICKY && Ball2.GetComponent<Ball>().GetBallType() == BallTypes.STICKY))
							PlayHitParticle();
						
						
					}
				}
				
			}
		}
		
		return t_Collisions.ToArray();
	}
	
	public BallTypes GetBallType()
	{
		return m_BallType;
	}
	
	public void SetCollide()
	{
		m_Collided = true;
		
	}
	
	void HandleBallCollisions()
	{
		if(!b_Active)
			return;
		
		GameObject[] t_Objects = CheckBallCollisions();
		
		Transform Ball2;
		Vector3 ball2Velocity;
		for(int i = 0; i < t_Objects.Length; i++)
		{						
			Ball2 = t_Objects[i].transform;
			CollisionPriorities Ball2Priority = Ball2.GetComponent<Ball>().GetCollisionPriority();
			if(m_PlayerSide!=Sides.NOSIDE) // Don't collide with balls on the side.
				continue;
			if(m_CollisionPriority > Ball2Priority) // if priority > ball, ignore collision
			{
				continue;
			}
			else
			{			
				//OVERWRITE collision with sticky ball behaviour -> stick to sticky ball if not sticky
				if(Ball2.GetComponent<Ball>().GetBallType() == BallTypes.STICKY) //ball2 is sticky
				{
				
					if(m_BallType != BallTypes.STICKY) //current ball is not sticky
					{
						m_Velocity = Ball2.GetComponent<Ball>().GetVelocity();
						//print("sticky");
						return;
					}
				}else if(m_BallType == BallTypes.STICKY)//ball2 not sticky, current ball sticky
				{
					m_Velocity = Ball2.GetComponent<Ball>().GetVelocity();
					return;
				}
				ball2Velocity = Ball2.GetComponent<Ball>().GetVelocity();
				
				transform.position = new Vector3(transform.position.x, m_Radius, transform.position.z);
				Ball2.position = new Vector3(Ball2.position.x, m_Radius, Ball2.position.z);
				
				Vector3 distanceVector = transform.position - Ball2.position;
				float distance = ((distanceVector).magnitude) - m_Radius*2;
				
				//check current ball against all other balls
				//set balls apart
				transform.position += (-m_Velocity.normalized)*(Mathf.Abs(distance));
				t_Objects[i].transform.position += (-ball2Velocity.normalized)*(Mathf.Abs(distance));
				
				//dot product of velocity against collision vector for both balls
				float dot1 = Vector3.Dot(ball2Velocity, distanceVector.normalized);
				float dot2 = Vector3.Dot(m_Velocity, distanceVector.normalized);
				
				//calculate delta V for both balls
				Vector3 deltaV1 = distanceVector.normalized*dot1;
				Vector3 deltaV2 = distanceVector.normalized*dot2;
				
				//assign new velocities
				m_Velocity += deltaV1 - deltaV2;
				
				
				if(GetBallType() == BallTypes.STICKY)
				PlayStickyCollisionSound(transform.position);
					else
				PlayBallCollisionSound(transform.position);
				
				//print (Ball2Priority);
				//print (m_CollisionPriority);
				if(m_CollisionPriority == Ball2Priority)// if equal in priority
				{
					Ball2.GetComponent<Ball>().SetVelocity(ball2Velocity+deltaV2-deltaV1);
				}
				
				m_Collided = true;
				Ball2.GetComponent<Ball>().SetCollide();
			}
		}
	}
	
	public void ResetBall()
	{
		StopBall();
		
		g_SlimeParticle.Stop();
		
		g_RocketParticle.Stop ();
		
		//ActivateRocketParticle(false);
		//m_RocketTrail.Stop(true);
		
		b_Active = false;
		
		m_CurveVector = new Vector3(0,0,0);
		
		transform.localRotation = Quaternion.identity;
		
		m_Collided = false;
		
		Vector3 tempNewPos = transform.position;
		//float positionZOffset = m_Radius;
		if(transform.position.z > 0.0f)
		{
			tempNewPos.z = Globals.g_FieldHeight - m_Radius;
		}
		else
		{
			tempNewPos.z = -Globals.g_FieldHeight + m_Radius;
		}
		
		tempNewPos.y = 0;
		
		transform.position = tempNewPos;
		b_SpecialActionDone = false;
		m_BallType = BallTypes.NORMAL;
		m_CollisionPriority = CollisionPriorities.LOW;
		m_StunTime = Globals.g_BallStunDefault;
		
				
		TrailRenderer tRender = GetComponent<TrailRenderer>();
		
		tRender.startWidth = 20.0f;
		tRender.endWidth = 1.0f;
	}
	
	public void RestartBall() //After a round ends
	{
		b_Held = false;
		
		TrailRenderer tRender = GetComponent<TrailRenderer>();
		
		tRender.enabled = false;
		
		m_HoldingPlayer = null;
		m_BallType = BallTypes.NORMAL;
		
		ResetBall ();
		if (m_InitialPosition.z != Globals.g_FieldHeight-m_Radius || m_InitialPosition.z != -Globals.g_FieldHeight+m_Radius)
		{
			b_Active = true;
			b_IgnoreBump = true;
		}
		this.transform.position = m_InitialPosition;
		
		m_PlayerSide = Sides.NOSIDE;
		
	}
	
	public void SetHeld(Player holdingPlayer)
	{
		TrailRenderer tRender = GetComponent<TrailRenderer>();
		
		tRender.enabled = true;
		b_Held = true;
		//if(Globals.g_CurrentGameModeStatic == Globals.GameModes.CHAOS)
		//{
		//	SetRandomType();
		//}
		m_HoldingPlayer = holdingPlayer;
		m_ParentPlayer = m_HoldingPlayer;
	}
	
	public CollisionPriorities GetCollisionPriority()
	{
		return m_CollisionPriority;
	}
		
	public void PlayHitParticle()
	{
		if (b_Held)
			return;
		
		ParticleSystem p;
		p = (ParticleSystem)Instantiate(Globals.g_ParticleHitStatic);
		
		p.transform.position = transform.position;
		p.Play();
	}

	public void PlayHitRedParticle()
	{
		if (b_Held)
			return;
		
		ParticleSystem p;
	
		p = (ParticleSystem)Instantiate(Globals.g_ParticleHitRedStatic);
		
		p.transform.position = transform.position;
		p.Play();
	}

	public void PlayHitRedParticle(Vector3 pos)
	{
		if (b_Held)
			return;
		
		ParticleSystem p;
		p = (ParticleSystem)Instantiate(Globals.g_ParticleHitRedStatic);
		
		p.transform.position = transform.position;
		p.Play();
	}

	public void PlayHitBlueParticle()
	{
		if (b_Held)
			return;
		
		ParticleSystem p;
		p = (ParticleSystem)Instantiate(Globals.g_ParticleHitBlueStatic);
		
		p.transform.position = transform.position;
		p.Play();
	}

	public void PlayShieldHitParticle()
	{
		if (b_Held)
			return;
		
		ParticleSystem p;
		p = (ParticleSystem)Instantiate(Globals.g_ParticleShieldHitStatic);
		
		p.transform.position = transform.position;
		p.Play();
	}
	
	public void PlaySpecialParticle(Color colour)
	{
		if (m_BallType == BallTypes.NORMAL)
			return;
		if (b_Held)
			return;
		
		ParticleSystem p;
		p = (ParticleSystem)Instantiate(Globals.g_ParticleSpecialStatic,transform.position,Quaternion.identity);


		p.startColor = colour;

		//Doesn't do anything
		GameObject.Find("BombExplosion").GetComponent<ParticleSystem>().startColor = colour;
		GameObject.Find("Explosion2").GetComponent<ParticleSystem>().startColor = colour;
		GameObject.Find("Flash").GetComponent<ParticleSystem>().startColor = colour;

	//	p.transform.position = transform.position;
	
		p.Play();
	}
	
	public void PlayPadParticle(Vector3 pos, Color colour)
	{
		if (b_Held)
			return;
		
		ParticleSystem p;
		p = (ParticleSystem)Instantiate(Globals.g_ParticlePadStatic);

		p.startColor = colour;

		GameObject.Find ("BombExplosion").GetComponent<ParticleSystem>().startColor = colour;
		GameObject.Find ("Explosion2").GetComponent<ParticleSystem>().startColor = colour;
		GameObject.Find ("Flash").GetComponent<ParticleSystem>().startColor = colour;

		p.transform.position = pos;
		p.Play();
	
	}
	
	private void PlayGravelParticle()
	{
	
	}

	
	private void StopGravelParticle()
	{
		if(m_Gravel!=null)
		{
			//m_Gravel.transform.position = new Vector3(-10000, 10000, -10000);
			m_Gravel.Stop();
		}
	}
	
	private void PlayExplosionParticle()
	{
		if (b_Held)
			return;
		
		ParticleSystem p;
		p = (ParticleSystem)Instantiate(Globals.g_ParticleExplosionStatic);
		
		//float scale = 2;
		
		//p.startSize *= scale;
		//p.startSpeed*= scale;

		p.transform.position = transform.position;

		p.Play();
	}
	
	private void PlayRocketParticle()
	{
		if (b_Held)
			return;
		
		ParticleSystem p;
		p = (ParticleSystem)Instantiate(Globals.g_RocketParticleStatic);
		
		p.transform.position = transform.position;
		p.Play();
	}
	
	void HandleBoundsCollision()
	{
		bool inBounds = EvaluatePointInBounds(transform.position);
		
		//get edge normal
		if(!inBounds)
		{
			float dist = 0.0f;
			Vector3 edgeNormal = GetClosestEdgeNormal(transform.position, ref dist);
	
			//set ball back
			transform.position += edgeNormal*dist;
	
			//i - (2 * n * dot(i, n)) 
			
			this.m_Velocity = m_Velocity-(2*edgeNormal*Vector3.Dot(m_Velocity,edgeNormal));
			
			m_Collided = true;
			
			//PlayBallCollisionSound(transform.position);
			if(GetBallType() == BallTypes.STICKY)
				PlayStickyCollisionSound(transform.position);
					else
				PlayBallCollisionSound(transform.position);
			
			PlayBallRollSound(transform.position);
			
			
			PlayHitParticle();
		}	
		
		if(transform.position.z > Globals.g_FieldHeight-m_Radius)
		{
			ResetBall(); // taken out for collision debug purposes, uncomment for gameplay
			m_PlayerSide = Sides.TOP;
			if(GetBallType() == BallTypes.STICKY)
				PlayStickyCollisionSound(transform.position);
					else
				PlayBallCollisionSound(transform.position);
			//m_Velocity.z *= -1; //DEBUG
		}else
		if(transform.position.z < -Globals.g_FieldHeight+m_Radius)
		{
			ResetBall(); // taken out for collision debug purposes, uncomment for gameplay	
			m_PlayerSide = Sides.BOTTOM;
			if(GetBallType() == BallTypes.STICKY)
				PlayStickyCollisionSound(transform.position);
					else
				PlayBallCollisionSound(transform.position);
			//m_Velocity.z *= -1; //DEBUG
		}else		
		if(transform.position.x > Globals.g_FieldWidth)
		{
			SetBallX(Globals.g_FieldWidth);
			//m_Velocity.x *= -1;
			//m_CurveVector *= -1;
			
			//PlayHitParticle();
			if(GetBallType() == BallTypes.STICKY)
				PlayStickyCollisionSound(transform.position);
					else
				PlayBallCollisionSound(transform.position);
			
			m_Collided = true;
		}else		
		if(transform.position.x < -Globals.g_FieldWidth)
		{
			SetBallX(-Globals.g_FieldWidth);
			//m_Velocity.x *= -1;
			//m_CurveVector *= -1;
			
			//PlayHitParticle();
			if(GetBallType() == BallTypes.STICKY)
				PlayStickyCollisionSound(transform.position);
					else
				PlayBallCollisionSound(transform.position);
			
			m_Collided = true;
		}		
	}
	
	private bool EvaluatePointInBounds(Vector3 pos)
	{
		bool outBool = false;
		int id = 0;
		//Vector3 origPos = pos;
		for(int i= 0 ; i < Globals.g_FieldPoints[Globals.g_LevelIDStatic].Length; i++)
		{
			id++;
			id %= Globals.g_FieldPoints[Globals.g_LevelIDStatic].Length;
			Vector3 currentPoint = Globals.g_FieldPoints[Globals.g_LevelIDStatic][i];
			currentPoint.x *= Globals.g_FieldWidth;
			currentPoint.z *= Globals.g_FieldHeight;
			Vector3 nextPoint = Globals.g_FieldPoints[Globals.g_LevelIDStatic][id];
			nextPoint.x *= Globals.g_FieldWidth;
			nextPoint.z *= Globals.g_FieldHeight;
	
			//source: http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
			if ((((currentPoint.z <= (pos.z)) && ((pos.z) < nextPoint.z)) || ((nextPoint.z <= (pos.z)) && ((pos.z) < currentPoint.z))) && ((pos.x) < (nextPoint.x - currentPoint.x) * ((pos.z) - currentPoint.z) / (nextPoint.z - currentPoint.z) + currentPoint.x))
			{
			  outBool = !outBool;
			}
		}
		
		return outBool;
	}
	
	private Vector3 GetClosestEdgeNormal(Vector3 point, ref float dist)
	{
		int id = 0; 
		Vector3[] closestSegment = new Vector3[2];
		float distance = 99999999.0f;
		float tempDistance;
		//find closest edge
		for(int i = 0; i < Globals.g_FieldPoints[Globals.g_LevelIDStatic].Length; i++)
		{
			id++;
			id %= Globals.g_FieldPoints[Globals.g_LevelIDStatic].Length;
			Vector3 currentPoint = Globals.g_FieldPoints[Globals.g_LevelIDStatic][i];
			currentPoint.x *= Globals.g_FieldWidth;
			currentPoint.z *= Globals.g_FieldHeight;
			Vector3 nextPoint = Globals.g_FieldPoints[Globals.g_LevelIDStatic][id];
			nextPoint.x *= Globals.g_FieldWidth;
			nextPoint.z *= Globals.g_FieldHeight;			
			
	
			tempDistance = DistancePointToSegment(point, currentPoint, nextPoint);
	
			if(tempDistance < distance)
			{
				distance = tempDistance;
				closestSegment[0] = currentPoint;
				closestSegment[1] = nextPoint;
			}
		}
		
		//printf("distance: %.2f\n",distance);
		//calc normal
		Vector3 normalizedSegment = (closestSegment[1]-closestSegment[0]);
		dist = distance;
	
		//NORMALIZE(normalizedSegment);
		normalizedSegment.Normalize();
		Vector3 crossResult = Vector3.Cross(new Vector3(0,1,0),normalizedSegment);
	
		//printf("crossResult: %.2f,%.2f,%.2f\n",crossResult.x,crossResult.y,crossResult.z);
		return crossResult;
	}
	
	private float DistancePointToSegment(Vector3 point, Vector3 s1, Vector3 s2)
	{
		//point = new Vector3(point.x, point.z, point.y);
	     Vector3 v = s2 - s1;
	     Vector3 w = point - s1;

	     float c1 = Vector3.Dot(w,v);
	     if ( c1 <= 0 )
	          return (point - s1).magnitude;
	
	     float c2 = Vector3.Dot(v,v);
	     if ( c2 <= c1 )
	          return (point - s2).magnitude;
	
	     float b = c1 / c2;
	     Vector3 Pb = s1 + b * v;
	     return (point - Pb).magnitude;
	}	
	
	public bool IsActive()
	{
		return b_Active;
	}
	
	GameObject CheckPlayerCollision()
	{
		GameObject[] Players = Globals.g_Players;
		
		for(int i = 0; i< Players.Length; i++)
		{
			Vector3 distance = transform.position - Players[i].transform.position;
			if(distance.magnitude < m_Radius+Globals.g_PlayerRadius)
			{
				return Players[i];
			}
		}
		return null;
	}
	
	void HandlePlayerCollision()
	{
		GameObject t_CollidingPlayer = CheckPlayerCollision();
			
		if(t_CollidingPlayer!=null)
		{
			//collision
			//int collidingPlayer = curPlayer.GetControllerID();
			Player t_Player = t_CollidingPlayer.GetComponent<Player>();
			
			if(b_Active && this.m_Velocity.magnitude > 0.0) // If the ball is moving
			{
				//stun player
				t_Player.AddStun(m_StunTime);

				PlayHitRedParticle(t_Player.transform.position);

			

				PlayPlayerHitSound(t_Player.transform.position,t_Player.GetCharacter() );
				
				//Register that the ball hit this side
				m_PlayerSide = t_Player.GetSide ();
				
				//reset ball
				ResetBall();
			}
			else
			{
				//ready to shoot
			}
		}
	}
	
	public Sides GetSide()
	{
		return m_PlayerSide;
	}
	
	private void FixYVelocity()
	{
		if(m_Velocity.y>0.0f)
		{
			float t_Len = m_Velocity.magnitude;
			m_Velocity.y = 0;
			m_Velocity.Normalize();
			m_Velocity *= t_Len;
		}
		
		transform.position = new Vector3(transform.position.x, m_Radius, transform.position.z);
	}
}
