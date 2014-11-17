using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpGuard: MonoBehaviour
{
	public int m_OwnerID = 0;//-1
	public int m_Lives = 1;
	private Vector3 m_Pos = Vector3.zero;
	private float m_Radius;
	private Vector3 m_Offset;
	
	private GameObject[] m_BallObjects;
	
	// Use this for initialization
	void Start ()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y+25,transform.position.z);
		m_Pos = transform.position;
		
		m_Radius = 15;
		
		GameObject owner = getOwner();
		if (owner != null)
			m_Offset = m_Pos - Globals.g_PlayerInitialPos[owner.GetComponent<Player>().GetID()];
		else
			new Vector3(transform.localScale.x*0.5f + Globals.g_Players[0].transform.localScale.x,0,transform.localScale.x*0.5f + Globals.g_Players[0].transform.localScale.x);

		if(m_OwnerID == 1)
		{
			transform.Rotate(0,180,0);
		}

		m_BallObjects = Globals.g_Balls;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (m_OwnerID != -1)
		{
			if (!isActive()) return;
			
			GameObject owner = getOwner();
			transform.position = owner.transform.position + m_Offset;

			HandleBoundsCollision(); //currently incorrect
			CheckCollision();
		}
	}
	
	void CheckCollision()
	{
		GameObject[] collisions = CheckBallCollisions();
		
		if (collisions.Length > 0)
		{
			handleBallCollisions();
			Destroy(this.gameObject);
		}
	}

	
	void HandleBoundsCollision()
	{	
		if(transform.position.x > Globals.g_FieldWidth)
		{
			transform.position = new Vector3(Globals.g_FieldWidth-1.0f,transform.position.y,transform.position.z);
		}
		else if(transform.position.x < -Globals.g_FieldWidth)
		{
			transform.position = new Vector3(-Globals.g_FieldWidth+1.0f,transform.position.y,transform.position.z);
		}
	}
	
	GameObject[] CheckBallCollisions()
	{
		
		List<GameObject> t_Collisions = new List<GameObject>();
		
		Transform Ball2;
		for(int i = 0; i < m_BallObjects.Length; i++)
		{			
			//check current ball against all other balls
			if(gameObject != m_BallObjects[i])
			{
				if(m_BallObjects[i].GetComponent<Ball>().GetVelocity().z > 0)
				{
					if(m_OwnerID == 0)
					{
						continue;
					}
				}else
				{
					if(m_OwnerID == 1)
					{
						continue;
					}
				}
				
				Ball2 = m_BallObjects[i].transform;
				
				if(Ball2.GetComponent<Ball>().IsActive())
				{
					//calculate distance
					Vector3 distanceVector = transform.position - Ball2.position;
					float distance = ((distanceVector).magnitude) - m_Radius*2;
					
					if(distance < 0) // ball radii already taken into account
					{
						t_Collisions.Add(m_BallObjects[i]);
						Ball2.GetComponent<Ball>().PlayShieldHitParticle();
					}
				}
				
			}
		}
		
		return t_Collisions.ToArray();
	}
	
	void handleBallCollisions()
	{
		GameObject[] t_Objects = CheckBallCollisions();
		
		Ball ballScript;
		Vector3 ballVelocity;
		for(int i = 0; i < t_Objects.Length; i++)
		{						
			ballScript = t_Objects[i].GetComponent<Ball>();
			ballVelocity = ballScript.GetVelocity();
			ballScript.SetVelocity(new Vector3(ballVelocity.x,ballVelocity.y,-ballVelocity.z));
			setOwnerID(getOwner().GetComponent<Player>().GetOpponent());
			ballScript.SetParentPlayer(Globals.g_Players[getOwner().GetComponent<Player>().GetOpponent()].GetComponent<Player>());
		}
	}
	
	public bool isActive()
	{
		if (m_Lives <= 0)
			return false;
		return true;
	}
	
	public void setOwner(Player player)
	{
		m_OwnerID = player.GetID();
	}
	
	public GameObject getOwner()
	{
		if (m_OwnerID != -1)
			if (Globals.g_Players[m_OwnerID] != null)
				return Globals.g_Players[m_OwnerID];
		return null;
	}
	
	public void setOwnerID(int ID)
	{
		m_OwnerID = ID;
	}
	
	public int getOwnerID()
	{
		return m_OwnerID;
	}
}
