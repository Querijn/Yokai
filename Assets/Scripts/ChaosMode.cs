using UnityEngine;
using System.Collections;

public class ChaosMode : MonoBehaviour 
{
	
	private float m_StartTime;
	private float m_CurrentTime = Globals.g_ChaosTime;
	private Camera m_Camera;
	private bool m_Paused = false;

	private float m_ScaleFactor;
	private float m_InitialXScale;
	

	// Use this for initialization
	void Start () 
	{
		m_StartTime = Globals.g_ChaosTime;//Time.time;
		m_Camera = GameObject.FindWithTag("MainCamera").camera.GetComponent<Camera>();
		m_Paused = false;

		m_ScaleFactor = 100.0f/m_StartTime;
		
		/*m_InitialXScale = this.guiTexture.pixelInset.width;

		Rect rect = this.guiTexture.pixelInset;

		rect.x = -(m_InitialXScale/2);

		this.guiTexture.pixelInset = rect;*/
	}

	// Update is called once per frame
	void Update () 
	{
		
		if(Globals.g_GamePaused)
		{
			m_Paused = true;
			
		}
		else
		{
			m_Paused = false;
			
		}
		
		if(Globals.g_GameHasEnded)
		{
			ResetTime();
			
		}
		
		if(!m_Camera.IsCinematicPlaying() && !m_Paused)
		{
			//float t_TimeDiff = (Time.time - m_StartTime);
			//m_CurrentTime = Mathf.Ceil(Globals.g_ChaosTime - t_TimeDiff);
			m_CurrentTime -= Time.deltaTime;
			
			if(m_CurrentTime<0.0)
			{
				
				int type = Random.Range(0, (int)Ball.BallTypes.MAX);
				
				while(type == (int)Ball.BallTypes.NORMAL)
				{
					type = Random.Range(0, (int)Ball.BallTypes.MAX);
				}
				
				for(int i = 0; i < Globals.g_TotalBalls; i++)
				{
					Ball ball = Globals.g_Balls[i].GetComponent<Ball>();

                    //only chaosify balls on sidelines
                    if (ball.IsActive())
                    {
                        continue;
                    }
					
					//print (i);
					
					
					
					ball.SetType(type);
					
					
					if(ball.IsActive() || ball.GetHeld())
						continue;
					
					
					Vector3 shootDir = new Vector3(0,0,0);
					
					if(ball.GetSide() == Ball.Sides.TOP)
						shootDir.z = -1;
					else
						shootDir.z = 1;
					
					shootDir.x = Random.Range (-1,1);
					
					shootDir.Normalize();
					
					
					
					ball.ShootBall(shootDir*Globals.g_ChaosBallLaunchSpeed, 15.0f);
					
					GameObject.FindWithTag("MainCamera").GetComponent<Camera>().Tremble(1);
				}
				
				
				for(int i = 0; i < 2; i++) //hardcoded 2 players
				{
					Player player = Globals.g_Players[i].GetComponent<Player>();
					
					
					if(player.GetBallsOnSide() < Globals.g_TotalBalls/2)
					{
						player.SpawnGuards();
						
					}
					
				}
				
				
				ResetTime();
			}
			
			//print (t_TimeDiff);
		}
		else m_StartTime = Time.time;
		
		int time = Mathf.CeilToInt(m_CurrentTime);
		
		this.guiText.text = "Chaos: "+time.ToString ();

		/*Rect rect = this.guiTexture.pixelInset;
		
		rect.width = m_CurrentTime * m_ScaleFactor * 0.01f;
		
		rect.width*= m_InitialXScale;

		this.guiTexture.pixelInset = rect;*/
	}
	
	public float GetTime()
	{
		return m_CurrentTime;
	}
	
	public void ResetTime()
	{
		m_StartTime = Globals.g_ChaosTime;//Time.time;
		m_CurrentTime = Globals.g_ChaosTime;
	}
}
