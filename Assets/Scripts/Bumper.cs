using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bumper : MonoBehaviour 
{
	
	private float m_Radius = 0;
	
	private float m_Scale = 1;
	 
	
	//Array of all the balls
	//private GameObject[] Globals.g_Balls;
	
	// Use this for initialization
	void Start () 
	{
		m_Radius = 8.0f;
		
		//m_InitialPosition = this.transform.position;
		//m_InitialPosition.z = Globals.g_FieldHeight/2+110;
		
		Globals.g_Balls = GameObject.FindGameObjectsWithTag("Ball");
		//transform.position = m_InitialPosition;

	}
	
	
	// Update is called once per frame
	void Update () 
	{
		//ball collisions check
		HandleBallCollisions();
		
		if(m_Scale > 1)
		{
			m_Scale -= Time.deltaTime*3;
			
		}
		else
			m_Scale = 1;
		transform.localScale = new Vector3(m_Scale, m_Scale,m_Scale);
	}
	
	private void PlayHitBlueParticle()
	{
		ParticleSystem p;
		p = (ParticleSystem)Instantiate(Globals.g_ParticleHitBlueStatic);
		
		p.transform.position = transform.position;
		p.Play();
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
						t_Collisions.Add(Globals.g_Balls[i]);
					}
				}
				
			}
		}
		
		return t_Collisions.ToArray();
	}
	
	void Hit()
	{
		if(m_Scale < 2.5f)
		m_Scale += 0.5f;
		
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
			float dot2 = Vector3.Dot(Vector3.zero, distanceVector.normalized);
			
			//calculate delta V for both balls
			Vector3 deltaV1 = distanceVector.normalized*dot1;
			Vector3 deltaV2 = distanceVector.normalized*dot2;
			if(Ball2.GetComponent<Ball>().GetVelocity().sqrMagnitude > 170000f)
			{
				Ball2.GetComponent<Ball>().SetVelocity((deltaV2-deltaV1).normalized*prevSpeed);
			}
			else
			{
			//Vector3 finalVel = new Vector3(ball2Velocity.x+m_Velocity.x, ball2Velocity.y, ball2Velocity.z*-1);
				Ball2.GetComponent<Ball>().SetVelocity((deltaV2-deltaV1).normalized*prevSpeed * Globals.g_BumperSpeedModifier);
			}
			//Ball2.GetComponent<Ball>().SetVelocity(finalVel);
			Ball2.GetComponent<Ball>().SetCollide();
			//assign new velocities
			//m_Velocity = ((deltaV1-deltaV2).normalized*Globals.g_BallSpeed).normalized*m_InitialVelocity.magnitude;
			//m_Velocity*=
			//print (m_Velocity);
			
			PlayHitBlueParticle();

			Ball2.GetComponent<Ball>().PlayPinballSound(transform.position);

			
			Hit ();
			
		}
	}
	
}
