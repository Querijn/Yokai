using UnityEngine;
using System.Collections;

public class PowerUpPad : MonoBehaviour 
{
	public float m_BoosMagnitude = 1.0f;
	public float m_BoostChance = 20.0f; // [0, 100] percentage
	public float m_GuardChance = 20.0f; // [0, 100] percentage
	
	private int m_PowerUpPadInitialDelay = 0; //These are set in manager
	private int m_PowerUpPadMinimumDelay = 0;
	private int m_PowerUpPadMaximumDelay = 0;

	private float m_Radius = 0;
	
	private float m_InitialDelay = 0;//(float)m_PowerUpPadInitialDelay;
	
	private float m_Delay = 0;//(float)m_PowerUpPadInitialDelay;
	
	private bool m_Active = false;
	
	private Color m_Colour;
	
	private int m_Type = 0; // 0 = boost, 1 = powerup
	private int m_PowerupID = 0;
	private int m_BoostDir = 1;
	
	private GameObject[] m_PadManager;
	
	void Awake()
	{
	

	}
	
	// Use this for initialization
	void Start () 
	{
		m_Radius = 10;
		
		//m_Colour = renderer.material.color;
		
		
		m_PadManager = GameObject.FindGameObjectsWithTag("PadManager");
		
		m_PowerUpPadInitialDelay = m_PadManager[0].GetComponent<PowerUpManager>().m_PowerUpPadInitialDelay;
		m_PowerUpPadMinimumDelay = m_PadManager[0].GetComponent<PowerUpManager>().m_PowerUpPadMinimumDelay;
		m_PowerUpPadMaximumDelay = m_PadManager[0].GetComponent<PowerUpManager>().m_PowerUpPadMaximumDelay;
		
		m_InitialDelay = (float)m_PowerUpPadInitialDelay;
		m_Delay = (float)m_PowerUpPadInitialDelay;
		
		
	}
	
	public bool GetActive()
	{
		return m_Active;
	}
	
	public void SetActive(bool active)
	{
		m_Active = active;
		PowerpadEffects t_Animation = this.GetComponent<PowerpadEffects>();
		t_Animation.Flip();
	}
	
	public void Reset()
	{
		m_Delay = Random.Range(m_PowerUpPadMinimumDelay,m_PowerUpPadMaximumDelay);
		m_Active = false;
	}
	
	void CheckActive()
	{
		if(m_Active == false)
		{
			if(!Globals.g_GamePaused && !Globals.g_GameHasEnded)
			m_Delay -= Time.deltaTime;
			
			if(m_Delay <= 0)
			{
				m_PadManager[0].GetComponent<PowerUpManager>().ActivateRandom();//Activate ();
				

				m_Delay = Random.Range(m_PowerUpPadMinimumDelay,m_PowerUpPadMaximumDelay);
			}
		}
		else
		{
			if(!Globals.g_GamePaused && !Globals.g_GameHasEnded)
			m_Delay -= Time.deltaTime;
			
			if(m_Delay <= 0)
			{
				m_Active = false;

				m_Delay = Random.Range(m_PowerUpPadMinimumDelay,m_PowerUpPadMaximumDelay);
			}
		}
	}
	
	[RPC]
	public void SendDelay(float delay)
	{
		m_Delay = delay;
	}
	
	public void Activate()
	{
		m_Active = true;
		int randReturn = Random.Range(0,100); //percentage
		
		///DEBUG
		//m_Type =1;
		//m_PowerupID = (int)Ball.BallTypes.CURVE;
		//return;
		///
		//boost pad
		//print ("Percentage Boost: " + randReturn);
		if(randReturn < m_BoostChance)
		{
			m_Type = 0;//boost
			//Vector3 ballVel = Globals.g_Balls[i].GetComponent<Ball>().GetVelocity();
			//booooooost
			m_BoostDir = (1-randReturn%2*2);
			//print((m_BoostDir>=1)?"Boost up":"Boost down");
			//ballVel = new Vector3(1.0f*ballVel.x,ballVel.y*1.0f,Mathf.Clamp((1.0f+(m_BoosMagnitude*boostDir)),0.2f,10.0f)*ballVel.z);
			//print (ballVel);
			//Globals.g_Balls[i].GetComponent<Ball>().SetVelocity(ballVel);
		}
		else //powerup or guard
		{
			randReturn = Random.Range(0,100);
			//print ("Percentage Guard: " + randReturn);
			if(randReturn < m_GuardChance)
			{
				m_Type = -1; //guard;
				//print ("guard");
			}
			else
			{	
				//powerup
				m_Type = 1;
				m_PowerupID = (int)Globals.GetGlobalObject().g_FieldPowerups[Random.Range(0, (int)Globals.GetGlobalObject().g_FieldPowerups.Length)];
				//m_PowerupID = (int)Ball.BallTypes.CURVE;
				//print ("pad powerup");
				//print ((Ball.BallTypes)m_PowerupID);
				////Globals.g_Balls[i].GetComponent<Ball>().GetParentPlayer().GiveSpecialToken(m_PowerupID);//2 = 
			}
		}

		
	}
	
	[RPC]
	public void SendType(int type, int boostDir, int powerUpID)
	{
		m_Type = type;
		m_BoostDir = boostDir;
		m_PowerupID = powerUpID;
	}
	
	public int GetType()
	{
		return m_Type;
	}
	
	public Ball.BallTypes GetPowerup() //RETURNS BALLTYPES.MAX WHEN not a powerup
	{
		if(m_Type <= 0) //boostpad or guard
		{
			return Ball.BallTypes.MAX;
		}else
		{
			return (Ball.BallTypes)(m_PowerupID);
		}
	}
	
	void UpdateColor()
	{
		//we cannot send a RPC using a color, so we use Vector3 instead
		Vector3 setColor = new Vector3(1,1,1);
		if (m_Active)
		{
			if(m_Type > 0) //powerup
			{
				Ball.BallTypes type = GetPowerup();
				//print (type);
				switch(type) //COLOR
				{
				case Ball.BallTypes.NORMAL:
					setColor = new Vector3(1.0f,1.0f,1.0f);
					break;
				case Ball.BallTypes.CURVE:
					setColor = new Vector3(0.8f,0.2f,0.7f);
					break;
				case Ball.BallTypes.ROCKET:
					setColor = new Vector3(0.8f,0.4f,0.2f);
					break;
				case Ball.BallTypes.HEAVY:
					setColor = new Vector3(0.6f,0.1f,0.2f);
					break;
				case Ball.BallTypes.LIGHT:
					setColor = new Vector3(0.8f,0.7f,0.15f);
					break;
				case Ball.BallTypes.STICKY:
					setColor = new Vector3(0.2f,0.85f,0.2f);
					break;
				case Ball.BallTypes.EXPLOSIVE:
					setColor = new Vector3(0.8f,0.15f,0.15f);
					break;
				}		
			}else if(m_Type == 0)//boost
			{
				setColor = new Vector3(1.0f,1.0f,0.0f);
			}else if(m_Type == -1) //guard
			{
				setColor = new Vector3(1.0f,0.0f,1.0f);
			}
		}
		else
			setColor = new Vector3(0.2f,0.2f,0.2f);

		PowerpadEffects t_Animation = this.GetComponent<PowerpadEffects>();
		if(m_Colour!=new Color(setColor.x,setColor.y,setColor.z))
		{
			m_Colour = new Color(setColor.x,setColor.y,setColor.z);
			t_Animation.Flip();
		}

		//renderer.material.color = new Color(setColor.x,setColor.y,setColor.z);
	}
	
	//we cannot send a RPC using a color, so we use Vector3 instead
	[RPC]
	public void SendColor(Vector3 color)
	{
		renderer.material.color = new Color(color.x,color.y,color.z,1.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckActive ();
		UpdateColor();
		
		if(m_Active == true)
		{
			for(int i = 0; i < Globals.g_Balls.Length; i++)
			{
				if(Globals.SphereCollision(this.transform.position, m_Radius, Globals.g_Balls[i].GetComponent<Ball>().transform.position, Globals.g_Balls[i].GetComponent<Ball>().GetRadius()))
				{
					//REMEMBER TO HAVE THE FIELD POWERUPS SET IN THE INSPECTOR FOR THE BALL PREFAB!
					//int randReturn = Random.Range(0,100); //percentage

					Globals.g_Balls[i].GetComponent<Ball>().PlayPadParticle(transform.position, m_Colour);
					Globals.g_Balls[i].GetComponent<Ball>().PlayPowerUpSound(transform.position);

					//boost pad
					if(m_Type == 0)
					{
						Vector3 ballVel = Globals.g_Balls[i].GetComponent<Ball>().GetVelocity();
						//booooooost
						//print((m_BoostDir>=1)?"Boost up":"Boost down");
						ballVel = new Vector3(1.0f*ballVel.x,ballVel.y*1.0f,Mathf.Clamp((1.0f+(m_BoosMagnitude*m_BoostDir)),0.2f,10.0f)*ballVel.z);
						Globals.g_Balls[i].GetComponent<Ball>().SetVelocity(ballVel);
					}else if(m_Type == 1)//powerup
					{
						//print ("pad powerup");
						//Globals.g_Balls[i].GetComponent<Ball>().GetParentPlayer().GiveSpecialToken();
						Player t_Player = Globals.g_Balls[i].GetComponent<Ball>().GetParentPlayer();
						if(t_Player!=null)
							t_Player.GiveSpecialToken(m_PowerupID);
					}else if(m_Type == -1)
					{
						//activate guard
						Player t_Player = Globals.g_Balls[i].GetComponent<Ball>().GetParentPlayer();
						if(t_Player!=null)
							t_Player.SpawnGuards();
					}
					
					m_Active = false;
					
						

					m_Delay = Random.Range(m_PowerUpPadMinimumDelay,m_PowerUpPadMaximumDelay);
				}
			}
		}
	}
	
	[RPC]
	public void SendActive(bool active)
	{
		m_Active = active;
	}
	
}
