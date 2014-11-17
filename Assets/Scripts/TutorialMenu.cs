using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialMenu : MonoBehaviour 
{
	public GUISkin m_GUISkin;
	
	public List<Texture2D> m_TutorialScreens = new List<Texture2D>();
	public Texture2D m_PrevButtonTex;
	public Texture2D m_NextButtonTex;
	public Texture2D m_BackButtonTex;
	
	private int m_CurrentScreen = 0;
	private Rect m_TexturePos = new Rect(0,0,Screen.width,Screen.height);
	
	private Rect m_PrevButton = new Rect(5,Screen.height-90,(Screen.height/3)+20,120);
	private Rect m_NextButton = new Rect(Screen.width-Screen.height/3-5,Screen.height-90,(Screen.height/3)+20,120);
	
	private Rect m_BackButton = new Rect(Screen.width*0.5f-Screen.height/6,Screen.height-90,(Screen.height/3)+20,120);

	public Texture2D m_PauseScreen;
	private Rect m_PauseRect = new Rect(0,0,Screen.width,Screen.height);
	private GUIStyle m_PauseStyle;

	void Start()
	{
		m_PauseStyle = new GUIStyle();
		m_PauseStyle.font = Globals.g_LoadingFontStatic;
		m_PauseStyle.fontSize = 40;
		m_PauseStyle.alignment = TextAnchor.MiddleCenter;

	}

	void OnGUI()
	{
		GUI.skin = m_GUISkin;
		
		GUI.DrawTexture(m_TexturePos, m_TutorialScreens[m_CurrentScreen]);
		
		if (m_CurrentScreen != 0)
		{
			if (GUI.Button(m_PrevButton,m_PrevButtonTex))
			{
				m_CurrentScreen--;
			}
		}
		if (m_CurrentScreen != m_TutorialScreens.Count-1)
		{
			if (GUI.Button(m_NextButton,m_NextButtonTex))
			{
				m_CurrentScreen++;
			}
		}
		if (GUI.Button(m_BackButton,m_BackButtonTex))
		{
			GUI.DrawTexture(m_PauseRect, m_PauseScreen);
			GUI.Label(m_PauseRect, "Please Wait...");
			Application.LoadLevel(0);
		}
	}
}
