using UnityEngine;
using System.Collections;

public class TimerScript : MonoBehaviour 
{
	private float m_StartTime;
	private float m_CurrentTime = Globals.g_GameTime;
	private Camera m_Camera;
	private float m_StoreTime = 0;
	
	
	private bool PauseTime = false;

	private float m_ScaleFactor;
	private float m_InitialXScale;
	

	void Start () 
	{
		m_StartTime = Time.time;
		m_Camera = GameObject.FindWithTag("MainCamera").camera.GetComponent<Camera>();
		m_StartTime = Globals.g_GameTime;
		m_CurrentTime = m_StartTime;

		/*
		this.guiText.fontSize = (int)(Screen.width/32f);

		Vector2 vec = this.guiText.pixelOffset;

		vec.y -=(Screen.width/420f);//blazeit

		this.guiText.pixelOffset = vec;
*/
		

		m_ScaleFactor = 100.0f/m_StartTime;

		/*m_InitialXScale = this.guiTexture.pixelInset.width;

		Rect rect = this.guiTexture.pixelInset;
		
		rect.x = -(m_InitialXScale/2);
		
		this.guiTexture.pixelInset = rect;*/

		m_StoreTime = 0;
	}
	
	public void PauseTimer(bool Pause)
	{
		PauseTime = Pause;
	}
	
	public void StoreTime()
	{
		m_StoreTime = m_CurrentTime;
	}
	
	public void ResetStoredTime()
	{
		m_StoreTime = 0;
	}
	
	void Update () 
	{
		if(!m_Camera.IsCinematicPlaying() )
		{
			
			if(!PauseTime)
			{
				/*float t_TimeDiff = (Time.time - m_StartTime);
				m_CurrentTime = Mathf.Ceil(Globals.g_GameTime - t_TimeDiff);
				if(m_CurrentTime<0.0)
				{
					// Time over
					m_CurrentTime = -1;
				}*/
				
				m_CurrentTime -= Time.deltaTime;
			}
		}
		else 
		{
			m_StartTime = Globals.g_GameTime;//Time.time;
		}
		
		int castTime = Mathf.CeilToInt(m_CurrentTime);
		
		this.guiText.text = "Time: " + castTime.ToString ();



		/*
		Rect rect = this.guiTexture.pixelInset;

		rect.width = m_CurrentTime * m_ScaleFactor * 0.01f;

		rect.width*= m_InitialXScale;

	//	rect.width-=664.0f;


		this.guiTexture.pixelInset = rect;*/

	}
	
	public float GetTime()
	{
		return m_CurrentTime;
	}
	
	public void ResumeTimer()
	{
	
	}
	
	
	public void ResetTime()
	{
		m_StartTime = Globals.g_GameTime;//Time.time;
		m_CurrentTime = Globals.g_GameTime;
	}
}
