using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Globals : MonoBehaviour 
{
	public enum GameModes
	{
		DEFAULT = 0,
		TUGOFWAR,
		CHAOS,
		PINBALL,
		MAX
	};
	
	public enum FieldTypes
	{
		DEFAULT = 0,
		HEX,
		TST,
		MAX
	};
	
	static Globals m_ThisObject;
	public static int g_PlayerSpeed = 180;
	public static float g_PlayerXLimit = 120;

	private float m_ButtonSize = 100.0f;

	public static int g_FieldHeight = 135;
	public static int g_FieldWidth = 130;
	public static float g_FieldBumpModifier = 0.05f;
	
	public static int g_WinningScore = 3;
	
	public static int g_TotalBalls = 10;
	
	public static float g_BallRadius = 6.5f;
	
	public static float g_BallStunDefault = 1.0f;
	
	public int g_LevelID = 0;
	public static int g_LevelIDStatic = 0;
	
	public static Vector3[][] g_FieldPoints = new Vector3[3][];
	public Ball.BallTypes[] g_FieldPowerups;
	
	public float g_zEnvironmentOffset = 150.0f;
	public float g_xEnvironmentOffset = 150.0f;	
	
	public static Material[] g_BallMaterials = new Material[(int)Ball.BallTypes.MAX];
	
	
	public static float g_BallSpeed = 220;//220;//150.0f;
	public static float g_ChaosBallLaunchSpeed = 110; 
	
	public static float g_GameTime = 60;
	public static float g_ObstacleTime = 60;
	
	
	public static float g_ChaosTime = 10;
	
	public static float g_PlayerRadius = 10.0f;
	
	public static float g_AimDeviationAngle = 80.0f;//60.0f;//40.0f;
	
	public static float g_RocketSpeed = 1.5f;
	
	public static float g_BumperSpeedModifier = 1.2f;
	
	//public static float g_MaxChargeTime = 0.3f;
	
	public static Vector3[] g_PlayerInitialPos = {new Vector3(0, 0, -g_FieldHeight),new Vector3(0,0,g_FieldHeight)}; // p1.z+fieldheight//public static Vector3[] g_PlayerInitialPos = {new Vector3(0, 0, -g_FieldHeight),new Vector3(0,0,g_FieldHeight)}; // p1.z+fieldheight
	
	public static ScoreController g_ScoreController = null;// = GameObject.FindGameObjectWithTag("ScoreController").GetComponent<ScoreController>();
	
	public static TimerScript g_Timer = null;
	
	public static GameObject[] g_Players = null;
	public static GameObject[] g_Balls = null;
	public static GameObject[] g_Obstacles = null;
	public static GameObject[] g_PowerPads = null;
	public static GameObject[] g_Territories = null;

	
	public static bool g_GamePaused = false;
	
	public GameObject g_Obstacle;
	public static float g_ObstacleOffset = 0.25f;
	
	public GameObject g_Post;
	public Material g_Wall;
	public GameObject g_TopEnvironment;
	public GameObject g_BottomEnvironment;
	public GameObject g_LeftEnvironment;
	public GameObject g_RightEnvironment;
	public static GameObject g_Camera = null;
	
	//public GameObject m_ObstacleRef;
	
	//private GameObject m_Obstacle;
	
	public static float g_ExplosionRadius = 100.0f;
	
	//private bool m_ObstacleActive = false;
	
	public static GameModes g_CurrentGameModeStatic = 0;
	
	public ParticleSystem g_ParticleHit;	
	public static ParticleSystem g_ParticleHitStatic;

	public ParticleSystem g_ParticleHitRed;	
	public static ParticleSystem g_ParticleHitRedStatic;

	public ParticleSystem g_ParticleHitBlue;	
	public static ParticleSystem g_ParticleHitBlueStatic;

	public ParticleSystem g_ParticleShieldHit;	
	public static ParticleSystem g_ParticleShieldHitStatic;

	public ParticleSystem g_GravelParticle;
	public static ParticleSystem g_GravelParticleStatic;
	
	public ParticleSystem g_RocketParticle;
	public static ParticleSystem g_RocketParticleStatic;
		
	public ParticleSystem g_ParticleExplosion;	
	public static ParticleSystem g_ParticleExplosionStatic;
	
	public ParticleSystem g_ParticlePad;	
	public static ParticleSystem g_ParticlePadStatic;
	
	public ParticleSystem g_ParticleSpecial;
	public static ParticleSystem g_ParticleSpecialStatic;

	public Font g_LoadingFont;
	public static Font g_LoadingFontStatic;

	public bool m_PlayVictorySoundOnce = true;
	

	public static bool g_IsNetworkStatic = false;
	
	public static bool g_GameHasEnded = false;
	
	public AudioSource m_MusicAudioSource;
	
	private bool[] playersAreReady = new bool[2];
	
	public Material g_PostMaterial;

	public GUISkin GUIskin;

	public Texture2D m_PauseScreen;
	private Rect m_PauseRect = new Rect(0,0,Screen.width,Screen.height);
	private bool m_LeavingLevel;

	private GUIStyle m_PauseStyle;

	public enum ButtonTex
	{
		CONTINUE = 0,
		RESTART,
		TOMENU
	};


	public Texture2D[] Buttons;
	
	public static int g_Winner = -1;
	public static float g_WinScreenTime = 3.0f;
	public static float g_WinScreenTimer = 0.0f;
	public static float g_WinScreenAngle = 180.0f;
	public static float g_WinScreenAngleIncrementInSeconds = g_WinScreenAngle/g_WinScreenTime;
	public static float g_WinScreenPrevAngle = 0.0f;
	
	private bool m_DisplayPlayerWinsText = false;
	public Texture2D m_Player1WinsRoundTex;
	public Texture2D m_Player1WinsGameTex;
	public Texture2D m_Player2WinsRoundTex;
	public Texture2D m_Player2WinsGameTex;
	public Texture2D m_DrawTex;
	
	private Rect m_WinningTexPos = new Rect(0,0,Screen.width/3,Screen.height/3);
	
	
	[RPC]
	void sendReady(int playerID, bool ready)
	{
		playersAreReady[playerID] = ready;
		print ("RPC send, player "+playerID.ToString());
	}
	
	IEnumerator waitForNetworkReady()
	{
		try
		{
			if (Network.isServer)
			{
				playersAreReady[1] = true;
				networkView.RPC ("sendReady",RPCMode.Others,1,true);
			}
			if (Network.isClient)
			{
				playersAreReady[0] = true;
				networkView.RPC ("sendReady",RPCMode.Others,0,true);
			}
		}
		catch{}
		yield return new WaitForSeconds(2.0f);
	}
	
	void Awake()
	{
		DontDestroyOnLoad(this);
		GameObject csHelper = GameObject.FindGameObjectWithTag("CSHelper");
		
		if (csHelper.GetComponent<CharacterSelectHelper>().g_GlobalsNetworkStatic)
		{
			while (!playersAreReady[0] && !playersAreReady[1])//Application.isLoadingLevel)
			{
				StartCoroutine(waitForNetworkReady());
				
				print("server ready"+playersAreReady[1]+" client ready"+playersAreReady[0]);
				
				if (playersAreReady[0] && playersAreReady[1])
				{
					print ("Both ready");
					break;
				}
			}
		}
		//g_IsNetworkStatic = csHelper.GetComponent<CharacterSelectHelper>().g_GlobalsNetworkStatic;
		g_LevelIDStatic = g_LevelID;
		
		g_ParticleHitStatic = g_ParticleHit;
		g_ParticleHitRedStatic = g_ParticleHitRed;
		g_ParticleHitBlueStatic = g_ParticleHitBlue;
		g_ParticleShieldHitStatic = g_ParticleShieldHit;
		g_GravelParticleStatic = g_GravelParticle;
		g_LoadingFontStatic = g_LoadingFont;
		
		g_RocketParticleStatic = g_RocketParticle;
		
		g_ParticlePadStatic = g_ParticlePad;
		
		g_ParticleSpecialStatic = g_ParticleSpecial;
		
		
		g_GamePaused = false;
		
		//initialise field points
		//level 0
		g_FieldPoints[1] = new Vector3[6]{ new Vector3(-0.5f,0.0f,1.0f), new Vector3(0.5f,0.0f,1.0f), new Vector3(1.00f,0.0f,0.0f), new Vector3(0.5f,0.0f,-1.0f), new Vector3(-0.5f,0.0f,-1.0f), new Vector3(-1.00f,0.0f,0)};
		g_FieldPoints[0] = new Vector3[12]{ new Vector3(-0.8f,0.0f,1.0f), new Vector3(0.8f,0.0f,1.0f), new Vector3(0.8f,0.0f,0.4f), new Vector3(1.0f,0.0f,0.2f), new Vector3(1.0f,0.0f,-0.2f), new Vector3(0.8f,0.0f,-0.4f), new Vector3(0.8f,0.0f,-1.0f), new Vector3(-0.8f,0.0f,-1.0f), new Vector3(-0.8f,0.0f,-0.4f),  new Vector3(-1.0f,0.0f,-0.2f), new Vector3(-1.0f,0.0f,0.2f), new Vector3(-0.8f,0.0f,0.4f)};
 		g_FieldPoints[2] = new Vector3[12]{ new Vector3(-1.0f,0.0f,1.0f), new Vector3(1.0f,0.0f,1.0f), new Vector3(1.0f,0.0f,0.7f), new Vector3(0.8f,0.0f,0.4f), new Vector3(0.8f,0.0f,-0.4f), new Vector3(1.0f,0.0f,-0.7f), new Vector3(1.0f,0.0f,-1.0f), new Vector3(-1.0f,0.0f,-1.0f), new Vector3(-1.0f,0.0f,-0.7f),  new Vector3(-0.8f,0.0f,-0.4f), new Vector3(-0.8f,0.0f,0.4f), new Vector3(-1.0f,0.0f,0.7f)};
		
		//print(Globals.g_FieldPoints[0][0]);
		
		g_ParticleExplosionStatic = g_ParticleExplosion;
		
		g_PlayerXLimit = Mathf.Abs(g_FieldPoints[g_LevelIDStatic][0].x)*g_FieldWidth;
		
		//ball materials
		g_BallMaterials[(int)Ball.BallTypes.NORMAL] = (Material)Resources.Load("Models/Materials/BallBlue",typeof(Material));//new Material("Models/Materials/GreenMat");
		g_BallMaterials[(int)Ball.BallTypes.LIGHT] = (Material)Resources.Load("Models/Materials/BallFast",typeof(Material));
		g_BallMaterials[(int)Ball.BallTypes.EXPLOSIVE] = (Material)Resources.Load("Models/Materials/BallExplosive",typeof(Material));
		g_BallMaterials[(int)Ball.BallTypes.CURVE] = (Material)Resources.Load("Models/Materials/BallCurve",typeof(Material));
		g_BallMaterials[(int)Ball.BallTypes.STICKY] = (Material)Resources.Load("Models/Materials/BallSticky",typeof(Material));
		g_BallMaterials[(int)Ball.BallTypes.ROCKET] = (Material)Resources.Load("Models/Materials/BallRocket",typeof(Material));
		g_BallMaterials[(int)Ball.BallTypes.HEAVY] = (Material)Resources.Load("Models/Materials/BallHeavy",typeof(Material));
		
	}
	// Use this for initialization
	void Start () 
	{
		m_ThisObject = this;
		g_ScoreController = GameObject.FindGameObjectWithTag("ScoreController").GetComponent<ScoreController>();
		g_Timer= GameObject.FindGameObjectWithTag("Timer").GetComponent<TimerScript>();
		g_Players = GameObject.FindGameObjectsWithTag("Player");
		g_Balls = GameObject.FindGameObjectsWithTag("Ball");
		g_Territories = GameObject.FindGameObjectsWithTag("Territory");
		
		g_LevelIDStatic = g_LevelID;
		
		g_GameHasEnded = false;

		m_LeavingLevel = false;

		m_PauseStyle = new GUIStyle();
		m_PauseStyle.font = Globals.g_LoadingFontStatic;
		m_PauseStyle.fontSize = 50;
		m_PauseStyle.alignment = TextAnchor.MiddleCenter;

		
		//Debug field, place a cube at point positions
		//debug line rendering		
		GameObject cube ;

		LineRenderer lRender = this.GetComponent<LineRenderer>();
		lRender.SetVertexCount(2);
		int length = 1;
		lRender.SetPosition(0,  new Vector3(g_FieldPoints[g_LevelIDStatic][0].x*g_FieldWidth,g_FieldPoints[g_LevelIDStatic][0].y,g_FieldPoints[g_LevelIDStatic][0].z*g_FieldHeight));
		lRender.SetPosition(1,  new Vector3(g_FieldPoints[g_LevelIDStatic][length].x*g_FieldWidth,g_FieldPoints[g_LevelIDStatic][length].y,g_FieldPoints[g_LevelIDStatic][length].z*g_FieldHeight));

		GameObject newLine = new GameObject("Line");
		LineRenderer lRend = newLine.AddComponent<LineRenderer>();
		lRend.material = lRender.material;
		lRend.SetWidth(10,10);

		lRend.SetVertexCount(2);
		int length2 = g_FieldPoints[g_LevelIDStatic].Length/2;
		lRend.SetPosition(0,  new Vector3(g_FieldPoints[g_LevelIDStatic][length2].x*g_FieldWidth,g_FieldPoints[g_LevelIDStatic][length2].y,g_FieldPoints[g_LevelIDStatic][length2].z*g_FieldHeight));
		lRend.SetPosition(1,  new Vector3(g_FieldPoints[g_LevelIDStatic][length2+1].x*g_FieldWidth,g_FieldPoints[g_LevelIDStatic][length2+1].y,g_FieldPoints[g_LevelIDStatic][length2+1].z*g_FieldHeight));


		for(int i = 0; i < g_FieldPoints[g_LevelIDStatic].Length; i++)
		{
			//cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube = (GameObject)Instantiate(g_Post);
			cube.transform.position = new Vector3(g_FieldPoints[g_LevelIDStatic][i].x*g_FieldWidth,g_FieldPoints[g_LevelIDStatic][i].y,g_FieldPoints[g_LevelIDStatic][i].z*g_FieldHeight);
			cube.transform.localScale *= 0.7f;
			cube.transform.LookAt(Vector3.zero);
			cube.renderer.material = g_PostMaterial;
			//cube.transform.Rotate(new Vector3(0,90,0));
//			lRender.SetPosition(i, cube.transform.position);
			
			//place posts back a bit
			//cube.transform.Rotate(new Vector3(0,-90,0));
			Vector3 placementvector = Vector3.Normalize(cube.transform.forward)*12.0f;
			placementvector.y = 0.0f;
			//cube.transform.position -= placementvector;
			//cube.transform.Rotate(new Vector3(0,90,0));


			if(i!=0 && i!=g_FieldPoints[g_LevelIDStatic].Length*0.5f)
			{
				int t_IndexNext = (i+1)%g_FieldPoints[g_LevelIDStatic].Length;

				ELSONODITO(i, t_IndexNext);
			}
		}

		//ELSONODITO(0, g_FieldPoints[g_LevelIDStatic].Length);

		//lRender.SetPosition(g_FieldPoints[g_LevelIDStatic].Length, 
		//new Vector3(g_FieldPoints[g_LevelIDStatic][0].x*g_FieldWidth,g_FieldPoints[g_LevelIDStatic][0].y,g_FieldPoints[g_LevelIDStatic][0].z*g_FieldHeight));

		//Music
		m_MusicAudioSource.Play();

		m_ButtonSize = (float)Screen.height / 3.0f;
		
		if (g_CurrentGameModeStatic == GameModes.TUGOFWAR)
		{
			for (int i = 0; i < 2; i++)
			{
				GameObject obstacle = (GameObject)GameObject.Instantiate(g_Obstacle);
				float offset = g_ObstacleOffset * g_FieldWidth;
				obstacle.transform.position = new Vector3(i* offset*2.0f-offset,0,0);
				obstacle.GetComponent<Obstacle>().m_InitialPosition = obstacle.transform.position;
			}
			g_Obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		}

		g_PowerPads = GameObject.FindGameObjectsWithTag("PowerPad");
		
		g_Camera = GameObject.FindGameObjectWithTag("MainCamera");
		
		g_IsNetworkStatic = GameObject.FindGameObjectWithTag("CSHelper").GetComponent<CharacterSelectHelper>().g_GlobalsNetworkStatic;
	}

	private void ELSONODITO(int i, int t_IndexNext)
	{
		Vector3 t_Left = new Vector3(g_FieldPoints[g_LevelIDStatic][i].x*g_FieldWidth, g_FieldPoints[g_LevelIDStatic][i].y+15, g_FieldPoints[g_LevelIDStatic][i].z*g_FieldHeight);
		Vector3 t_Right = new Vector3(g_FieldPoints[g_LevelIDStatic][t_IndexNext].x*g_FieldWidth, g_FieldPoints[g_LevelIDStatic][t_IndexNext].y+15, g_FieldPoints[g_LevelIDStatic][t_IndexNext].z*g_FieldHeight);
		
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = new Vector3[] 
		{	
			t_Left-new Vector3(0.0f, 25, 0.0f),
			t_Right-new Vector3(0.0f, 25, 0.0f),
			t_Right+new Vector3(0.0f, 35, 0.0f),
			t_Left+new Vector3(0, 35, 0.0f)
		};
		m.uv = new Vector2[] {
			
			new Vector2 (0, 1),
			new Vector2 (1, 1),
			new Vector2 (1, 0),
			new Vector2 (0, 0)
		};
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
		m.RecalculateNormals();
		GameObject plane = new GameObject("Thomas The Tank Engine was a good show alright");
		MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
		meshFilter.mesh = m;
		MeshRenderer t_Renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		
		plane.renderer.materials.SetValue(g_Wall, 0);
		plane.renderer.material = g_Wall;
	}
	
	[RPC]
	public void SendEndGame(int id)
	{
		if(id != -1)
			Globals.g_ScoreController.IncreaseForPlayer(id);
		
		Globals.g_Timer.ResetTime();
	}
	// Update is called once per frame
	void Update () 
	{

		if(g_CurrentGameModeStatic == GameModes.TUGOFWAR)
		{
			//Now tug of war is a timed mode
			//g_Timer.ResetTime(); //So that the time doesn't decrease
		}
		CheckEndGame();
		
		if(g_ScoreController.HasPlayerWon() != -1)
		{
			//ID of winning player
			g_Winner = g_ScoreController.HasPlayerWon();
			
			g_GameHasEnded = true;
		}

		if(g_GameHasEnded)
		{
			g_Timer.ResetTime();
			g_ScoreController.Reset();
		}
			

		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if(g_GamePaused)
			{
				g_GamePaused = false;
				g_Timer.ResumeTimer();
				print("Pause");
				Time.timeScale = 1.0f;
			}
			else
			{
				g_Timer.StoreTime();
				g_GamePaused = true;
				Time.timeScale = 0.0f;
			}
		}
	}
	
	public static Globals GetGlobalObject()
	{
		return m_ThisObject;
	}
	
	void OnGUI()
	{
		GUI.skin = GUIskin;

		PausePlayers(false);
		g_Timer.PauseTimer(false);

		if(m_DisplayPlayerWinsText)
		{
			//this.guiText.enabled = true;
			
			m_WinningTexPos.center = new Vector2(Screen.width/2,Screen.height-Screen.height/6);
			
			if (g_Winner == 0)
			{
				if (g_GameHasEnded)
					GUI.DrawTexture(m_WinningTexPos, m_Player1WinsGameTex);
				else
					GUI.DrawTexture(m_WinningTexPos, m_Player1WinsRoundTex);
			}
			else if (g_Winner == 1)
			{
				if (g_GameHasEnded)
					GUI.DrawTexture(m_WinningTexPos, m_Player2WinsGameTex);
				else
					GUI.DrawTexture(m_WinningTexPos, m_Player2WinsRoundTex);
			}
			else if (g_Winner == -2)
				GUI.DrawTexture(m_WinningTexPos,m_DrawTex);
		}
		
		if(g_GameHasEnded)
		{
			PausePlayers(true);
			
			DisplayEndgameGUI(g_Winner);
			
			g_Timer.PauseTimer(true);
		}
		
		if(g_GamePaused)
		{
			PausePlayers(true);

			if(g_GameHasEnded==false)
			DisplayPauseMenu();
			
			g_Timer.PauseTimer(true);
		}

		if(m_LeavingLevel)
		{
			GUI.DrawTexture(m_PauseRect, m_PauseScreen);
			GUI.Label(m_PauseRect, "Please Wait...", m_PauseStyle);
		}
	
	}
	
	void DisplayPauseMenu()
	{
		//GUILayout.BeginArea(new Rect(Screen.width*0.5f-620*0.25f, Screen.height*0.5f+400*0.125f, 
		//							 Screen.width*0.5f+620*0.5f,  Screen.height*0.5f+400*0.5f)//new Rect(Screen.width/2 - 64, 64, 256,Screen.height-64)
		//							);

		GUILayout.BeginArea(new Rect((Screen.width*0.5f)-(m_ButtonSize*(3.0f*0.5f)),(Screen.height*0.5f)+64, Screen.width - 64, Screen.height - 64));
		//GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();

		float buttonSize = 200;
		
		if(GUILayout.Button(Buttons[(int)ButtonTex.CONTINUE]/*"Continue Game"*/, GUILayout.Width(m_ButtonSize/*620*0.5f*/), GUILayout.Height(buttonSize)) )
		{
			g_GamePaused = false;
			Time.timeScale = 1.0f;
			PausePlayers(false);
			g_Timer.ResumeTimer();

		}
		
		if(GUILayout.Button(Buttons[(int)ButtonTex.RESTART]/*"Restart Game"*/, GUILayout.Width(m_ButtonSize), GUILayout.Height(buttonSize)) )
		{
			ResetChaosMode();
			g_GameHasEnded = false;
			g_GamePaused = false;
			Time.timeScale = 1.0f;
			
			PausePlayers(false);
			for(int i = 0; i < 2; i++)
			{
				
				g_Players[i].GetComponent<Player>().ResetPlayer();
			}
			
			for(int i = 0; i< g_TotalBalls; i++)
			{
				g_Balls[i].GetComponent<Ball>().RestartBall();
			}
			
			g_ScoreController.Reset();
			
			g_Timer.PauseTimer(false);
			g_Timer.ResetTime();
			
			
			g_Camera.GetComponent<Camera>().m_PlayCinematic = true;
			g_Camera.GetComponent<Camera>().ResetCameraTimer();
			
			if(Globals.g_Obstacles != null)
			{
				for(int i = 0; i < g_Obstacles.Length; i++)
					Globals.g_Obstacles[i].GetComponent<Obstacle>().ResetObstacle();

				for(int i = 0; i < g_Territories.Length; i++)
					Globals.g_Territories[i].GetComponent<Territory>().Reset();
			}

		}
		
		if(GUILayout.RepeatButton(Buttons[(int)ButtonTex.TOMENU]/*"Go to Menu"*/,  GUILayout.Width(m_ButtonSize), GUILayout.Height(buttonSize)) )
		{
			g_GamePaused = false;
			Time.timeScale = 1.0f;
			g_GameHasEnded = false;

			ReturnToMainMenu();
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
		
		
	}



	void DisplayEndgameGUI(int winner)
	{
		//GUILayout.BeginArea(new Rect(Screen.width*0.5f-620*0.25f, Screen.height*0.5f+400*0.125f, 
		//							 Screen.width*0.5f+620*0.5f,  Screen.height*0.5f+400*0.5f)//new Rect(Screen.width/2 - 64, 64, 256,Screen.height-64)
		//							);

		GUILayout.BeginArea(new Rect((Screen.width*0.5f)-(m_ButtonSize*(2*0.5f)),(Screen.height*0.5f) -32, Screen.width - 64, Screen.height - 128));
		
		GUILayout.BeginHorizontal();
		
		float buttonSize = 200;
		
		if(GUILayout.Button(Buttons[3], GUILayout.Width(m_ButtonSize), GUILayout.Height(buttonSize)) )
		{
			
			ResetChaosMode();
			g_GameHasEnded = false;
			g_Winner = -1;
			g_WinScreenTimer = 0.0f;
			g_WinScreenPrevAngle = 0.0f;
			g_ScoreController.Reset();
			g_Camera.GetComponent<Camera>().Reset();
			
			PausePlayers(false);
			for(int i = 0; i < 2; i++)
			{
			
				g_Players[i].GetComponent<Player>().ResetPlayer();
			}
			
			for(int i = 0; i< g_TotalBalls; i++)
			{
				g_Balls[i].GetComponent<Ball>().RestartBall();
			}
			
			g_ScoreController.Reset();
			
			g_Timer.PauseTimer(false);
			g_Timer.ResetTime();
			
			g_Camera.GetComponent<Camera>().m_PlayCinematic = true;
			g_Camera.GetComponent<Camera>().ResetCameraTimer();
			
			if(Globals.g_Obstacles != null)
			{
				for(int i = 0; i < g_Obstacles.Length; i++)
					Globals.g_Obstacles[i].GetComponent<Obstacle>().ResetObstacle();

				for(int i = 0; i < g_Territories.Length; i++)
					Globals.g_Territories[i].GetComponent<Territory>().Reset();

			}

		}
		
		if(GUILayout.RepeatButton(Buttons[4], GUILayout.Width(m_ButtonSize), GUILayout.Height(buttonSize)) )
		{
			g_GameHasEnded = false;
			g_Winner = -1;
			g_WinScreenTimer = 0.0f;
			g_WinScreenPrevAngle = 0.0f;
			ReturnToMainMenu();
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
		
	}
	
	public void PausePlayers(bool paused)
	{
		
		for(int i = 0; i < 2; i++)
		{
			g_Players[i].GetComponent<Player>().EndGame(paused);
		}
	}
	public void CheckEndGame()
	{
		if (Globals.g_CurrentGameModeStatic != GameModes.TUGOFWAR)
		{
			// when balls are on one side
			foreach (GameObject player in Globals.g_Players)
			{
				player.GetComponent<Player>().CountBalls();
				if(player.GetComponent<Player>().GetBallsOnSide() >= Globals.g_TotalBalls)
				{
					g_Winner = player.GetComponent<Player>().GetOpponent();
				}
			}
		}
		// timer runs out
		if(Globals.g_Timer.GetTime() < 0)
		{
			bool a_Draw = true;
			int id = -1;

			if(g_CurrentGameModeStatic == GameModes.TUGOFWAR)
			{
				int score0 = 0;
				int score1 = 0;

				for(int i = 0; i < g_Territories.Length; i++)
				{
					Territory t = g_Territories[i].GetComponent<Territory>();

					if(t.GetOwner() != null && t.GetOwner().GetID() == 0)
					{
						score0++;
					}
					else if(t.GetOwner() != null && t.GetOwner().GetID() == 1)
					{
						score1++;
					}


					if(score0 > score1)
					{
						a_Draw = false;
						id = Globals.g_Players[0].GetComponent<Player>().GetID();
					}
					else if(score0 < score1)
					{
						a_Draw = false;
						id = Globals.g_Players[1].GetComponent<Player>().GetID();

					}else
					{
						a_Draw = true;
						id = -2;
					}

				}

			}
			else
			{
			
				for (int i = 0; i<2; i++)
				{
					Player p = Globals.g_Players[i].GetComponent<Player>();
					p.CountBalls();
				}
				if (Globals.g_Players[0].GetComponent<Player>().GetBallsOnSide() < Globals.g_Players[1].GetComponent<Player>().GetBallsOnSide())
				{
					a_Draw = false;
					id = Globals.g_Players[0].GetComponent<Player>().GetID();
				}else if (Globals.g_Players[0].GetComponent<Player>().GetBallsOnSide() > Globals.g_Players[1].GetComponent<Player>().GetBallsOnSide())
				{
					a_Draw = false;
					id = Globals.g_Players[1].GetComponent<Player>().GetID();
				}
			}
			
			if(!a_Draw)
			{
				g_Winner = id;
			}
			else
			{
				g_Winner = -2; //draw

			}
		}
		if (g_Winner != -1 && g_Winner != -2)
		{
			if (g_WinScreenTimer <= g_WinScreenTime)
			{
				PausePlayers(true);
				g_Players[g_Winner].GetComponent<Player>().DestroyGuards();
				
				m_DisplayPlayerWinsText = true;
				
				if (g_Winner == 0)
					g_WinScreenPrevAngle += g_WinScreenAngleIncrementInSeconds * Time.deltaTime;
				else
					g_WinScreenPrevAngle -= g_WinScreenAngleIncrementInSeconds * Time.deltaTime;
				
				if (g_GameHasEnded || g_ScoreController.GetScore(g_Winner) == 2)
				{
					Globals.g_ScoreController.IncreaseForPlayer(g_Winner);
					if (g_Winner == 0)
						g_WinScreenPrevAngle = 90.0f;
					else
						g_WinScreenPrevAngle = 270.0f;
					// add particles/cool background here
				}
				
				g_Camera.GetComponent<Camera>().PlayPlayerVictoryCinametic(g_Winner,g_WinScreenPrevAngle*Mathf.Deg2Rad);



				//Make this play once
				if(m_PlayVictorySoundOnce == true)
				{
					m_MusicAudioSource.Pause();
					g_Players[g_Winner].GetComponent<Player>().PlayVictorySound(g_Players[g_Winner].transform.position);
				}
				m_PlayVictorySoundOnce = false;

				if (!g_GameHasEnded)
					g_WinScreenTimer += Time.deltaTime;
				if (g_CurrentGameModeStatic == GameModes.CHAOS)
				{
					GameObject.FindGameObjectWithTag("EnableChaosMode").GetComponent<ChaosMode>().ResetTime();
				}
				g_Timer.ResetTime();



				return;
			}
			else
			{
				m_PlayVictorySoundOnce = true;
				m_MusicAudioSource.Play();
				m_DisplayPlayerWinsText = false;
				PausePlayers(false);
				g_WinScreenPrevAngle = 0.0f;
				g_WinScreenTimer = 0.0f;
				g_Camera.GetComponent<Camera>().Reset();
			}
			PausePlayers(true);
			Globals.g_ScoreController.IncreaseForPlayer(g_Winner);
			g_Winner = -1;
		}
		else if(g_Winner == -2) //Draw
		{
			if (g_WinScreenTimer <= g_WinScreenTime)
			{
				PausePlayers(true);
				m_DisplayPlayerWinsText = true;
				
				if (!g_GameHasEnded)
					g_WinScreenTimer += Time.deltaTime;
				
				if (g_CurrentGameModeStatic == GameModes.CHAOS)
				{
					GameObject.FindGameObjectWithTag("EnableChaosMode").GetComponent<ChaosMode>().ResetTime();
				}
				g_Timer.ResetTime();
				return;
			}
			else
			{
				m_DisplayPlayerWinsText = false;
				PausePlayers(false);
				g_WinScreenPrevAngle = 0.0f;
				g_WinScreenTimer = 0.0f;
				g_Camera.GetComponent<Camera>().Reset();
			}
			m_PlayVictorySoundOnce = true;
			PausePlayers(true);
			g_Winner = -1;
		}
		else
			return;
		for(int i = 0; i < 2; i++)
			Globals.g_Players[i].GetComponent<Player>().ResetPlayer();
		
		if(Globals.g_Obstacles != null && Globals.g_CurrentGameModeStatic == Globals.GameModes.TUGOFWAR)
		{
			for(int i = 0; i < Globals.g_Obstacles.Length; i++)
			{
				Globals.g_Obstacles[i].GetComponent<Obstacle>().ResetObstacle();
			}

			for(int i = 0; i < g_Territories.Length; i++)
				Globals.g_Territories[i].GetComponent<Territory>().Reset();
		}
		
		//g_Camera.GetComponent<Camera>().m_PlayCinematic = true;
		//g_Camera.GetComponent<Camera>().ResetCameraTimer();
		
		//Reset everything
		for(int i = 0; i < Globals.g_TotalBalls; i++)
		{
			Globals.g_Balls[i].GetComponent<Ball>().RestartBall();
		}
		Globals.g_Timer.ResetTime();
		
		for(int i = 0; i < Globals.g_PowerPads.Length; i++)
		{
			Globals.g_PowerPads[i].GetComponent<PowerUpPad>().Reset();
			
		}
		ResetChaosMode();
	}
	
	public void ResetChaosMode()
	{
		if(g_CurrentGameModeStatic == GameModes.CHAOS)
		{
			GameObject chaosObject = GameObject.FindGameObjectWithTag("EnableChaosMode");
			
			chaosObject.GetComponent<ChaosMode>().ResetTime();
		}
	}
	
	public void ReturnToMainMenu()
	{
		//delete globals
		m_MusicAudioSource.Stop();
		
		GameObject.Destroy(m_ThisObject);
	
		//delete lines
		LineRenderer lRender = this.GetComponent<LineRenderer>();
		if(lRender != null)
		{
			lRender.SetVertexCount(0);
		}
	
		m_LeavingLevel = true;

		Application.LoadLevel(0);		
	}
	
	public static bool SphereCollision(Vector3 pos1, float r1, Vector3 pos2, float r2)
	{
		Vector3 d = pos1 - pos2;
		float r = r1 + r2;
	
		if( (d.x*d.x) + (d.y*d.y) + (d.z*d.z) < (r*r) )
		{
			return true;
		}
		
		return false;
	}
}
