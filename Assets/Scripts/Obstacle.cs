using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour 
{
	private Vector3 m_Velocity = new Vector3(0,0,0);
	private Vector3 m_InitialVelocity = new Vector3(0,0,0);
	public Vector3 m_InitialPosition = new Vector3(0,0,0);
	public static float m_ObstacleWeight = 0.03f;
	
	private float m_Radius = 0;
	
	private PowerUp m_PowerUp;
	
	// last player that hit this obstacle
	private Player m_Parent = null;
	
	private enum PowerUp
	{
		LIGHT = 0,
		NORMAL,
		EXPLOSIVE,
		POWER,
		CURVE,
		STICKY,
		ROCKET,
		HEAVY,
		MAX
	};
	
	//Array of all the balls
	//private GameObject[] Globals.g_Balls;
	
	// Use this for initialization
	void Start () 
	{
		m_Radius = 8.0f;//transform.localScale.x*0.5f;
		
		//m_InitialPosition = this.transform.position;
		//m_InitialPosition.z = Globals.g_FieldHeight/2+110;
		
		Globals.g_Balls = Globals.g_Balls;
		//transform.position = m_InitialPosition;
		
		m_Velocity = m_InitialVelocity;
		
		ResetType();
	}

	
	void ResetType()
	{
		m_PowerUp = PowerUp.NORMAL;
		while(m_PowerUp == PowerUp.NORMAL)
		{
			int t_Random = Random.Range(0,(int)PowerUp.MAX);
			m_PowerUp = (PowerUp)t_Random;
		}
		
		//print (this.renderer.material.color);
		//this.renderer.material.SetVector("_Color", Globals.g_Balls[0].GetComponent<Ball>().GetBallTypeColour((Ball.BallTypes)((int)m_PowerUp)));	
		//print (this.renderer.material.color);
		
		this.renderer.material.SetFloat("_Show", 0.0f);
	}
	
	public void ResetObstacle()
	{
		transform.position= m_InitialPosition;
		m_Velocity = Vector3.zero;
		renderer.material.color = new Color(1f,1f,1f,1f);
		m_Parent = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Globals.g_GamePaused)
			return;
		
		this.transform.position = new Vector3(transform.position.x, 0,transform.position.z);
		m_Velocity *= 0.99f;
		this.transform.position += m_Velocity * Time.deltaTime * m_ObstacleWeight;
	
		//HandlePowerUpCode();
		
		//ball collisions check
		HandleBallCollisions();
		
		//Bounds check
		HandleBoundsCollision();
		
		HandleObstacleCollision();
		
		if (m_Parent != null)
		{
			if (m_Parent.GetID() == 0)
				renderer.material.color = new Color(1f,0f,0f,1f);
			else if (m_Parent.GetID() == 1)
				renderer.material.color = new Color(0f,0f,1f,1f);
		}
	}
	
	void HandleObstacleCollision()
	{
		for (int i = 0; i < Globals.g_Obstacles.Length; i++)
		{
			GameObject t_obstacle = (GameObject)Globals.g_Obstacles[i];
			
			if (t_obstacle == this.gameObject)
				continue;
			
			Vector3 t_obstacleVelocity = t_obstacle.GetComponent<Obstacle>().m_Velocity;
			
			Vector3 distanceVector = transform.position - t_obstacle.transform.position;
			float distance = ((distanceVector).magnitude) - m_Radius*2;
			
			if(distance < 0)
			{
				float prevSpeed = t_obstacleVelocity.magnitude;
				
				//check current ball against all other balls
				//set balls apart
				Globals.g_Obstacles[i].transform.position += (-t_obstacleVelocity.normalized)*(Mathf.Abs(distance));
				
				//dot product of velocity against collision vector for both balls
				float dot1 = Vector3.Dot(t_obstacleVelocity, distanceVector.normalized);
				float dot2 = Vector3.Dot(m_Velocity, distanceVector.normalized);
				
				//calculate delta V for both balls
				Vector3 deltaV1 = distanceVector.normalized*dot1;
				Vector3 deltaV2 = distanceVector.normalized*dot2;
					
				//Vector3 finalVel = new Vector3(ball2Velocity.x+m_Velocity.x, ball2Velocity.y, ball2Velocity.z*-1);
				t_obstacle.GetComponent<Obstacle>().SetVelocity((deltaV2-deltaV1).normalized*prevSpeed*2);
				//assign new velocities
				m_Velocity = ((deltaV1-deltaV2).normalized*prevSpeed)*2;
				//print (m_Velocity);
			}
		}
	}
	
	public void SetVelocity(Vector3 velocity)
	{
		m_Velocity = velocity;
	}
	
	void HandlePowerUpCode()
	{
		float t_DistVecFromCenter = transform.position.z-m_InitialPosition.z;
		
		//m_Velocity = ((m_Velocity.x<0)?(-m_InitialVelocity):(m_InitialVelocity)) * (1+Mathf.Abs(t_DistVecFromCenter)*0.03f); // Adjust speed
		
		foreach(GameObject t_Player in Globals.g_Players)
		{
			if(Mathf.Abs(t_Player.transform.position.z-transform.position.z)<30)
			{
				Globals.g_Players[t_Player.GetComponent<Player>().GetOpponent()].GetComponent<Player>().GiveSpecialToken((int)m_PowerUp);
				//this.transform.position = m_InitialPosition;
				m_Velocity = m_InitialVelocity;
				ResetType();
			}
		}
	}
	
	[RPC]
	void SetPosition(Vector3 pos, Vector3 vel)
	{
		transform.position = pos;
		m_Velocity = vel;
	}
	
	void HandleBoundsCollision()
	{
		bool inBounds = EvaluatePointInBounds(transform.position);
		
		//get edge normal
		if(!inBounds)
		{
			Vector3 edgeNormal = GetClosestEdgeNormal(transform.position);
	
			//set ball back
			transform.position += edgeNormal*m_Radius*Time.deltaTime;
	
			//i - (2 * n * dot(i, n)) 
			
			this.m_Velocity = m_Velocity-(2*edgeNormal*Vector3.Dot(m_Velocity,edgeNormal));
			
			//m_Collided = true;
			
			//PlayHitParticle();
		}	

		if(transform.position.z > Globals.g_FieldHeight-m_Radius)
		{
			//ResetBall(); // taken out for collision debug purposes, uncomment for gameplay
			//m_PlayerSide = Sides.TOP;
			m_Velocity.z = -1500;// *= -5; //DEBUG //This is what fucks it up
			//ResetObstacle();
			//print (m_Velocity.z);

		}
		else
		if(transform.position.z < -Globals.g_FieldHeight+m_Radius)
		{
			//ResetBall(); // taken out for collision debug purposes, uncomment for gameplay	
			//m_PlayerSide = Sides.BOTTOM;
			
			m_Velocity.z = 1500;// *= -5; //DEBUG
			//ResetObstacle();
			//print (m_Velocity.z);

		}	
	}
	
	GameObject[] CheckBallCollisions()
	{
		List<GameObject> t_Collisions = new List<GameObject>();
		
		Transform Ball2;
		if(Globals.g_Balls!=null)
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
					
					if(distance < 0)
					{
						t_Collisions.Add(Globals.g_Balls[i]);
						Globals.g_Balls[i].GetComponent<Ball>().PlayHitParticle();
						Globals.g_Balls[i].GetComponent<Ball>().PlayBallCollisionSound(transform.position);
					}
				}
				
			}
		}
		
		return t_Collisions.ToArray();
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
	
	private Vector3 GetClosestEdgeNormal(Vector3 point)
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
	
	void HandleBallCollisions()
	{	
		GameObject[] t_Objects = CheckBallCollisions();
		
		Transform Ball2;
		Vector3 ball2Velocity;
		for(int i = 0; i < t_Objects.Length; i++)
		{						
			Ball2 = t_Objects[i].transform;
			ball2Velocity = Ball2.GetComponent<Ball>().GetVelocity();
			
			Vector3 distanceVector = transform.position - Ball2.position;
			float distance = ((distanceVector).magnitude) - m_Radius*2;
			
			float prevSpeed = ball2Velocity.magnitude;
			
			//check current ball against all other balls
			//set balls apart
			//transform.position += (-m_Velocity.normalized)*(Mathf.Abs(distance));
			t_Objects[i].transform.position += (-ball2Velocity.normalized)*(Mathf.Abs(distance));
			
			
			//dot product of velocity against collision vector for both balls
			float dot1 = Vector3.Dot(ball2Velocity, distanceVector.normalized);
			float dot2 = Vector3.Dot(m_Velocity, distanceVector.normalized);
			
			//calculate delta V for both balls
			Vector3 deltaV1 = distanceVector.normalized*dot1;
			Vector3 deltaV2 = distanceVector.normalized*dot2;

			//Vector3 finalVel = new Vector3(ball2Velocity.x+m_Velocity.x, ball2Velocity.y, ball2Velocity.z*-1);
			Ball2.GetComponent<Ball>().SetVelocity((deltaV2-deltaV1).normalized*prevSpeed);
			//Ball2.GetComponent<Ball>().SetVelocity(finalVel);
			Ball2.GetComponent<Ball>().SetCollide();
			//assign new velocities

			m_Velocity = ((deltaV1-deltaV2).normalized*prevSpeed)*4.2f; //This handles the acceleration
			
			// set parent to ball parent
			m_Parent = Ball2.GetComponent<Ball>().GetParentPlayer();
		}
	}
	
	public Player GetParent()
	{
		return m_Parent;
	}
	
	void SetX(float a_Value)
	{
		
	}
	
	int GetPowerUp()
	{
		return (int)m_PowerUp;
	}
	
	void SetPowerUp(PowerUp a_PowerUp)
	{
		m_PowerUp = a_PowerUp;
	}
}
