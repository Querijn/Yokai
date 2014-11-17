using UnityEngine;
using System.Collections;

public class Territory : MonoBehaviour 
{
	public Ball.BallTypes m_PowerupType = Ball.BallTypes.CURVE;
	public float m_CaptureTime = 2.0f;
	private float m_CaptureTimer;
	
	private Player m_Owner = null;
	private static Texture[] m_Textures = new Texture[7];
		
	
	// Use this for initialization
	void Start ()
	{
		m_CaptureTimer = m_CaptureTime;
		transform.GetChild(0).renderer.material.color = new Color(1f,1f,1f,0.5f);
		
		m_Textures[(int)0] = (Texture)Resources.Load("UI/wood_D", typeof(Texture));
		m_Textures[(int)1] = (Texture)Resources.Load("UI/light_ball_icon", typeof(Texture));
		m_Textures[(int)2] = (Texture)Resources.Load("UI/explosive_ball_icon", typeof(Texture));
		m_Textures[(int)3] = (Texture)Resources.Load("UI/curve_ball_icon", typeof(Texture));
		m_Textures[(int)4] = (Texture)Resources.Load("UI/sticky_ball_icon", typeof(Texture));
		m_Textures[(int)5] = (Texture)Resources.Load("UI/rocket_ball_icon", typeof(Texture));
		m_Textures[(int)6] = (Texture)Resources.Load("UI/heavy_ball_icon", typeof(Texture));
		
		renderer.material.SetTexture("_MainTex",m_Textures[(int) m_PowerupType]);
	}

	public Player GetOwner()
	{
		return m_Owner;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Globals.g_GamePaused || Globals.g_GameHasEnded)
			return;
		
		bool intersection = false;
		Player obstacleParent = null;
		foreach (GameObject o in Globals.g_Obstacles)
		{
			if (!transform.GetChild(0).collider.bounds.Intersects(o.collider.bounds))
				continue;
			
			if (o.GetComponent<Obstacle>().GetParent() != null)
			{
				intersection = true;
				obstacleParent = o.GetComponent<Obstacle>().GetParent();
			}
		}
		
		if (intersection)
		{
			if (obstacleParent != null)
			{
				m_CaptureTimer -= Time.deltaTime;
				
				if (m_CaptureTimer <= 0.0f)
				{
					if (m_Owner != obstacleParent)
					{
						Globals.g_Balls[0].GetComponent<Ball>().PlayPowerUpSound(transform.position);
						obstacleParent.GiveSpecialToken((int) m_PowerupType);
					}
						
					m_Owner = obstacleParent;
					m_CaptureTimer = m_CaptureTime;
				}
			}
		}else
			m_CaptureTimer = m_CaptureTime;
		
		if (m_Owner != null)
		{
			if (m_Owner.GetID() == 0)
				transform.GetChild(0).renderer.material.color = new Color(1f,0f,0f,0.5f);
			else if (m_Owner.GetID() == 1)
				transform.GetChild(0).renderer.material.color = new Color(0f,0f,1f,0.5f);
		}
	}
	
	public void Reset()
	{
		m_Owner = null;
		m_CaptureTimer = m_CaptureTime;
		transform.GetChild(0).renderer.material.color = new Color(1.0f,1.0f,1.0f,0.5f);
	}
}
