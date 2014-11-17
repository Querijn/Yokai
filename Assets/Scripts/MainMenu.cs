using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
	private float m_ButtonSize = 100;
	private float m_ButtonWallOffset = 45;
	public CharacterSelectHelper m_CSHelper;
	public List<string> m_sceneNames = new List<string>();
	public List<string> m_fieldNames = new List<string>();
	private int m_MenuType = 0;
	private string m_levelName = "";
	private string m_LevelToLoadName = "";
	public List<Texture> m_Logos = new List<Texture>();
	public Texture2D[] m_HoverTextures = new Texture2D[7];
	private int m_CurrentLogo = 0;
	private bool b_Once = false;
	
	//for name
	private string m_Name = "";
	private int nameNum = 0;
	private int gName = 0;
	private int hName = 0;
	private int iName = 0;			
	
	private string[] m_BattleBallBrawl = new string[3];
	
	public List<GameObject> m_Characters;
	public List<GameObject> m_CharacterSpawn;
	GameObject t_Objects;
	private bool m_Spawned = false;
	
	private bool[] m_PlayerSelected = new bool[2];
	private Rect[] m_PlayerSelectedBounds = new Rect[2];
	public List<Texture> m_PlayerSelectedTextures = new List<Texture>();
	
	private float m_NumActiveButtons = 1;
	
	public AudioSource m_MenuMusic;
	public AudioSource m_ButtonSound;
	
	public Texture2D[] Buttons;
	
	public GUISkin GUIskin;

	public Texture2D m_PauseScreen;
	private Rect m_PauseRect = new Rect(0,0,Screen.width,Screen.height);
	private bool m_LeavingLevel;

	private GUIStyle m_PauseStyle;

	public Font m_PauseFont;

	
	public enum ButtonTex
	{
		NORMAL = 0,
		TUG,
		CHAOS,
		PINBALL,
		BADGER,
		CHICKEN,
		CONTINUE,
		CREDITS,
		EXIT,
		LEVEL1,
		LEVEL2,
		LEVEL3,
		LOCAL,
		LOST,
		NETWORK,
		OPTIONS,
		PLAY,
		RETURN,
		SELECT,
		MAX
		
		
	};
	
	private string m_ConnectToIP = "127.0.0.1";
	private int m_ConnectPort = 25000;
	
	private bool[] m_ReadyToLoad = new bool[2];
	private bool b_once = false;
	
	// Use this for initialization
	void Start () 
	{
		m_BattleBallBrawl[0] = "Battle";
		m_BattleBallBrawl[1] = "Brawl";
		m_BattleBallBrawl[2] = "Ball";
		
		m_MenuMusic.Play();
		
		m_ButtonSize = (float)Screen.height / 3.0f;
		
		m_ReadyToLoad[0] = false;
		m_ReadyToLoad[1] = false;
		
		m_PlayerSelected[0] = false;
		m_PlayerSelected[1] = false;
		
		m_CSHelper = GameObject.FindGameObjectWithTag("CSHelper").GetComponent<CharacterSelectHelper>();

		m_LeavingLevel = false;

		m_PauseStyle = new GUIStyle();
		m_PauseStyle.font = m_PauseFont;
		m_PauseStyle.fontSize = 50;
		m_PauseStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//print (Network.connections);
		bool startLevel = false;
		for (int i = 0; i < 2; i++)
		{
			if (!m_ReadyToLoad[i])
			{
				startLevel = false;
				break;
			}
			startLevel = true;
		}/*
		if (startLevel)
		{
			print ("level start");
			startLevel = false;
			Application.LoadLevel(m_LevelToLoadName);
		}*/
	}
	
	[RPC]
	void setPlayerReady(int playerID, bool ready)
	{
		m_ReadyToLoad[playerID] = ready;
		print("player "+playerID+" ready");
	}
	
	[RPC]
	void setCharacter(int playerID, int characterID)
	{
		m_CSHelper.g_PlayerCharacterID[playerID] = characterID;
		print ("character "+ characterID + " selected for player "+ playerID);
	}
	
	[RPC]
	void setLevel(string levelname)
	{
		m_LevelToLoadName = levelname;
		print (" level set to " + levelname);
	}
	
	void OnGUI () 
	{
		GUI.skin = GUIskin;

	//	1366x598



		float logoWidth = Screen.width*0.5f;//620;
		float logoHeight = Screen.height*0.7f;//400;
	
		GUI.DrawTexture(new Rect(Screen.width*0.5f-logoWidth*0.5f, 0, logoWidth, logoHeight), m_Logos[m_CurrentLogo]);  // Draw Logo
		/*GUILayout.BeginArea(new Rect(Screen.width*0.5f-620*0.25f, Screen.height*0.5f+400*0.125f, 
									 Screen.width*0.5f+620*0.5f,  Screen.height*0.5f+400*0.5f)//new Rect(Screen.width/2 - 64, 64, 256,Screen.height-64)
									);*/
		
		if (m_PlayerSelected[0])
			GUI.DrawTexture(m_PlayerSelectedBounds[0],m_PlayerSelectedTextures[0]);
		if (m_PlayerSelected[1])
			GUI.DrawTexture(m_PlayerSelectedBounds[1],m_PlayerSelectedTextures[1]);
		
		if(!b_Once)
		{
			b_Once = true;
			gName = Random.Range(1,3);
			hName = (gName==1)?2:0;
			iName = (hName==2)?0:1;
			m_Name += m_BattleBallBrawl[nameNum+iName] + " " + m_BattleBallBrawl[(nameNum+gName)%3] + " " + m_BattleBallBrawl[(nameNum+hName)%3];
		}
		
		//GUILayout.BeginArea(new Rect(Screen.width/2 - 64 - 50, (Screen.height*0.5f) + 64, 256,Screen.height-64));
		GUILayout.BeginArea(new Rect((Screen.width*0.5f)-(m_ButtonSize*(m_NumActiveButtons*0.5f)),(Screen.height*0.5f) + 64, Screen.width - 64, Screen.height - 64));
		//GUILayout.TextField(m_Name,GUILayout.Width(Screen.width*0.2f));
		
		if (m_MenuType == 0 && !m_CSHelper.g_GlobalsNetworkStatic)
		{
			m_NumActiveButtons = 3.0f;
			m_CurrentLogo = 0;
			
			GUILayout.BeginHorizontal();
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.PLAY]/*"Play"*/, GUILayout.Width(m_ButtonSize/*620*0.5f*/), GUILayout.Height(200)) )
			{
				m_MenuType = 2;
				m_ButtonSound.Play();
			}

			if(GUILayout.RepeatButton(Buttons[(int)ButtonTex.OPTIONS]/*"Tutorial"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_LeavingLevel = true;
				Application.LoadLevel("TutorialScene");
			}
			
		//	if(GUILayout.Button(Buttons[(int)ButtonTex.CREDITS]/*"Credits"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				
			}
			
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.EXIT]/*"Quit"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_ButtonSound.Play();
				Application.Quit();
			}
	
			
			GUILayout.EndHorizontal();


			
		}
		else if (m_MenuType == 1 && !m_CSHelper.g_GlobalsNetworkStatic)
		{
			m_NumActiveButtons = 3.0f;
			
			GUILayout.BeginHorizontal();
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.LOCAL]/*"Local"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_ButtonSound.Play();
				m_CSHelper.g_GlobalsNetworkStatic = false;
				m_MenuType = 2;
			}
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.NETWORK]/*"Network"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_ButtonSound.Play();
				m_CSHelper.g_GlobalsNetworkStatic = true;
				m_MenuType = -1;
			}
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.RETURN]/*"Back"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_ButtonSound.Play();
				m_MenuType = 0;
			}
	
			
			GUILayout.EndHorizontal();
		}
		else if (m_MenuType == -1)
		{
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				m_ConnectToIP = GUILayout.TextField(m_ConnectToIP, GUILayout.Width(620*0.5f));
				m_ConnectPort = int.Parse(GUILayout.TextField(m_ConnectPort.ToString(),GUILayout.Width(620*0.5f)));
				
				if (GUILayout.Button ("Connect as client", GUILayout.Width(620*0.5f), GUILayout.Height(m_ButtonSize)) )
				{
					//Connect to the "connectToIP" and "connectPort" as entered via the GUI
					Network.useNat = false;
					Network.Connect(m_ConnectToIP, m_ConnectPort);
					Application.LoadLevel("ConnectionScene");
				}
				
				if (GUILayout.Button ("Start Server", GUILayout.Width(620*0.5f), GUILayout.Height(m_ButtonSize)) )
				{
					//Start a server for 2 clients using the "connectPort" given via the GUI
					Network.useNat = false;
					Network.InitializeServer(1, m_ConnectPort);
					Application.LoadLevel("ConnectionScene");
				}
			}
		}
		else if (m_MenuType == 2 && !m_CSHelper.g_GlobalsNetworkStatic ||
				(m_MenuType == 2 && m_CSHelper.g_GlobalsNetworkStatic && Network.isServer) )
		{			
			GUILayout.BeginHorizontal();
			m_CurrentLogo = 1;
			
			m_NumActiveButtons = 4.0f + 1.0f;
			
			//print((int)Globals.GameModes.MAX);
			
			for (int i = 0; i < (int)Globals.GameModes.MAX; i++)
			{
				if(GUILayout.Button(Buttons[i]/*"Play "+m_sceneNames[i]*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
				{
					m_ButtonSound.Play();
					Globals.GameModes gameMode = (Globals.GameModes)i;
					Globals.g_CurrentGameModeStatic = gameMode;
					m_MenuType = 3;
					m_levelName = m_sceneNames[i];
				}
				
				// hover event
				if(Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition ))
				{

					Rect lastRect = GUILayoutUtility.GetLastRect();
                    Rect origRect = lastRect;
					
                    //lastRect.center -= new Vector2(0, 100);
                    //lastRect.size *= 0.5f;

                    lastRect.height -= 100.0f;
                    lastRect.position -= new Vector2(0, 25);
                    

                    //GUILayout.BeginArea(lastRect);
                    GUI.DrawTexture(lastRect, m_HoverTextures[3 + i]);
                    //GUILayout.EndArea();
				}
			}
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.RETURN]/*"Back"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_ButtonSound.Play();
				m_MenuType = 0;
			}
	
			
			GUILayout.EndHorizontal();
		}
		else if (m_MenuType ==  3&& !m_CSHelper.g_GlobalsNetworkStatic ||
				(m_MenuType ==  3&& m_CSHelper.g_GlobalsNetworkStatic && Network.isServer) )
		{
			GUILayout.BeginHorizontal();
			m_CurrentLogo = 2;
			
			if (m_Spawned)
			{
				foreach (GameObject c in m_CharacterSpawn)
				{
					Destroy(c);
				}
				m_CharacterSpawn.Clear();
				m_Spawned = false;
			}
			
			m_NumActiveButtons = 3.0f+1.0f;
			
			for (int i = 0; i < (int)Globals.FieldTypes.MAX; i++)
			{
				if(GUILayout.Button(Buttons[i+9]/*m_fieldNames[i]*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
				{
					//print(m_levelName+"_"+m_fieldNames[i]+".unity");
					m_LevelToLoadName = m_levelName+"_"+m_fieldNames[i];
					//Globals.g_LevelIDStatic = i;
					//print ("level static: " + Globals.g_LevelIDStatic);
					m_ButtonSound.Play();
					m_MenuType = 4;
				}
				// hover event
				if(Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition ))
				{

					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.y = lastRect.yMin;
					GUI.DrawTexture(lastRect,m_HoverTextures[i]);
				}
			}
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.RETURN]/*"back"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_ButtonSound.Play();
				m_MenuType = 2;
			}
	
			
			GUILayout.EndHorizontal();
		}
		else if (m_MenuType ==  4&& !m_CSHelper.g_GlobalsNetworkStatic ||
				(m_MenuType ==  4&& m_CSHelper.g_GlobalsNetworkStatic && Network.isServer) )// player one character select
		{
			m_PlayerSelected[1] = false;
			m_CurrentLogo = 3;
			
			m_NumActiveButtons = 2.0f;
			GUILayout.BeginHorizontal();
			
			if(m_Spawned==false)
			{
				for (int i = 0; i < m_Characters.Count; i++)
				{
					Vector3 t_Pos = new Vector3(i*80.0f-40.0f, -25.0f, 50.0f);
					GameObject m_Prefab = (GameObject)Instantiate(m_Characters[i], t_Pos, Quaternion.LookRotation(new Vector3(0.0f, 0.0f, -1.0f)));
					m_Prefab.transform.position = t_Pos;
					m_Prefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
					m_CharacterSpawn.Add (m_Prefab);
					//m_Prefab.transform.Rotate(0, 180, 0);
				}
				m_Spawned = true;
			}
			for (int i = 0; i < m_Characters.Count; i++)
			{
				if(GUILayout.Button(Buttons[i+4]/*m_Characters[i].name.ToString()*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
				{
					m_CSHelper.g_PlayerCharacterID[1] = i;
					m_MenuType = 5;

					m_ButtonSound.Play();
					
					m_PlayerSelectedBounds[1] = new Rect(i*(Screen.width-m_ButtonSize)/* - i*155.0f*/,0.0f,m_ButtonSize,200);
					m_PlayerSelected[1] = true;
					m_ReadyToLoad[1] = true;
					if (m_CSHelper.g_GlobalsNetworkStatic)
					{
						networkView.RPC("setPlayerReady",RPCMode.Others,1,true);
						networkView.RPC("setCharacter",RPCMode.Others,1,i);
						networkView.RPC("setLevel",RPCMode.Others,m_LevelToLoadName);
					}
				}
				m_CharacterSpawn[i].transform.localRotation = Quaternion.LookRotation(new Vector3(0.0f, 0.0f, Mathf.Sin (Time.time)));
			}
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.RETURN]/*"back"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_ButtonSound.Play();
				m_MenuType = 3;
			}
			
			GUILayout.EndHorizontal();
		}
		else if (m_MenuType ==  5&& !m_CSHelper.g_GlobalsNetworkStatic ||
				(m_MenuType ==  5&& m_CSHelper.g_GlobalsNetworkStatic && Network.isClient) )// player two character select
		{
			m_PlayerSelected[0] = false;
			
			if(m_Spawned==false)
			{
				for (int i = 0; i < m_Characters.Count; i++)
				{
					Vector3 t_Pos = new Vector3(i*80.0f-40.0f, -25.0f, 50.0f);
					GameObject m_Prefab = (GameObject)Instantiate(m_Characters[i], t_Pos, Quaternion.LookRotation(new Vector3(0.0f, 0.0f, -1.0f)));
					m_Prefab.transform.position = t_Pos;
					m_Prefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
					m_CharacterSpawn.Add (m_Prefab);
					//m_Prefab.transform.Rotate(0, 180, 0);
				}
				m_Spawned = true;
			}
			m_NumActiveButtons = 2.0f;
			
			GUILayout.BeginHorizontal();
			//GUI.Box(GUILayoutUtility.GetRect(m_ButtonSize,200),m_PlayerSelectedTextures[0]);//, GUILayout.Width(m_ButtonSize), GUILayout.Height(200));
			for (int i = 0; i < m_Characters.Count; i++)
			{
				if(GUILayout.Button(Buttons[i+4]/*m_Characters[i].name.ToString()*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
				{
					m_ButtonSound.Play();
					m_CSHelper.g_PlayerCharacterID[0] = i;
					m_MenuType = 6;
					print("P1: " + m_Characters[m_CSHelper.g_PlayerCharacterID[1]].name);
					print("P2: " + m_Characters[m_CSHelper.g_PlayerCharacterID[0]].name);
					//print (m_ReadyToLoad);
					
					m_PlayerSelectedBounds[0] = new Rect(i*(Screen.width-m_ButtonSize)/*-(i-1.0f)*155.0f*/,150.0f,m_ButtonSize,200.0f);
					m_PlayerSelected[0] = true;
					m_ReadyToLoad[0] = true;
					if (m_CSHelper.g_GlobalsNetworkStatic)
					{
						networkView.RPC("setPlayerReady",RPCMode.Others,0,true);
						networkView.RPC("setCharacter",RPCMode.Others,0,i);
					}
				}
				m_CharacterSpawn[i].transform.localRotation = Quaternion.LookRotation(new Vector3(0.0f, 0.0f, Mathf.Sin (Time.time)));
			}
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.RETURN]/*"back"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_ButtonSound.Play();
				m_MenuType = 4;
			}
			GUILayout.EndHorizontal();
		}
		else if (m_MenuType == 6)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(Buttons[(int)ButtonTex.PLAY], GUILayout.Width(m_ButtonSize), GUILayout.Height(200)))
			{
				m_ButtonSound.Play();
				m_LeavingLevel = true;
				Application.LoadLevel(m_LevelToLoadName);
			}
			
			if(GUILayout.Button(Buttons[(int)ButtonTex.RETURN]/*"back"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(200)) )
			{
				m_ButtonSound.Play();
				m_MenuType = 5;
			}
			GUILayout.EndHorizontal();
		}
		else if (m_CSHelper.g_GlobalsNetworkStatic && Network.isServer)
			m_MenuType = 2;
		else if (m_CSHelper.g_GlobalsNetworkStatic && Network.isClient)
			m_MenuType = 5;
	
		GUILayout.EndArea();

		if(m_LeavingLevel)
		{
			GUI.DrawTexture(m_PauseRect, m_PauseScreen);

			GUI.Label(m_PauseRect, "Please Wait...", m_PauseStyle);
		}
		
		
	}
}
