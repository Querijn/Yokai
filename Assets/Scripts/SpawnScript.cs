using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnScript: MonoBehaviour
{
	public GameObject m_Player;
	public List<Player> m_PlayerScripts = new List<Player>();
	
	public GameObject m_Ball;
	private int m_BallsToSpawn = 0;
	
	void Start()
	{

		GameObject h = (GameObject)GameObject.FindGameObjectWithTag("CSHelper");
		CharacterSelectHelper helper = h.GetComponent<CharacterSelectHelper>();
		SpawnBalls();
		for (int i = 0; i < 2; i++)
		{
//				print ("player " +i.ToString()+ " spawning character: "+helper.g_PlayerCharacter[helper.g_PlayerCharacterID[i]].name);
			GameObject playerGO = null;
			playerGO = (GameObject)GameObject.Instantiate(m_Player);
			playerGO.GetComponent<Player>().SetID(i);
			playerGO.GetComponent<Player>().SetCharacter(helper.g_PlayerCharacterID[i]);
		}
		Globals.g_Players = GameObject.FindGameObjectsWithTag("Player");
//			print ("Global players: "+Globals.g_Players[0].name+" / "+Globals.g_Players[1].name);
	}
	
	void OnServerInitialized()
	{
		if (Globals.g_IsNetworkStatic)
		{
			//SpawnBalls();
			SpawnPlayer(Network.player);
		}
	}
	
	void OnPlayerConnected(NetworkPlayer newPlayer)
	{
		if (Globals.g_IsNetworkStatic)
		{
			SpawnPlayer(newPlayer);
			SpawnBalls();
			ResetBalls();
		}
		Globals.g_Timer.GetComponent<TimerScript>().ResetTime();
	}
	
	void SpawnPlayer(NetworkPlayer newPlayer)
	{
		int playerNum = int.Parse(newPlayer+"");
		//print(playerNum);
		//print ("Player Created: " +playerNum);
		//set the camera
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().m_target = playerNum;
		
		//instantiate a new playerobject for the new player
		CharacterSelectHelper helper = GameObject.FindGameObjectWithTag("CSHelper").GetComponent<CharacterSelectHelper>();
		GameObject playerGO = (GameObject)Network.Instantiate(helper.g_PlayerCharacter[helper.g_PlayerCharacterID[playerNum]],transform.position,transform.rotation,playerNum);
		playerGO.AddComponent<Player>();
		//add the mesh to the player
		//playerGO.GetComponent<MeshFilter>().mesh = m_playerMeshes[m_playerMeshID];
		
		//update the players in the globals
		Globals.g_Players = GameObject.FindGameObjectsWithTag("Player");
		
		//set the players ID
		playerGO.GetComponent<Player>().SetID(playerNum);
		
		Transform player = playerGO.transform;
		
		NetworkView playerNetView = player.networkView;
		
		//keep track of the player
		m_PlayerScripts.Add(player.GetComponent<Player>());
		
		//call a RPC on the new networkview, set the networkplayer who controls this new player
		playerNetView.RPC("SetPlayer",RPCMode.AllBuffered, newPlayer);
	}
	
	void SpawnBalls()
	{
		GameObject[] sceneBalls = GameObject.FindGameObjectsWithTag("Ball");
			//for (int i = 0; i < sceneBalls.Length; i++)
			//	sceneBalls[i].GetComponent<Ball>().b_Active = true;
			
		m_BallsToSpawn = Globals.g_TotalBalls - sceneBalls.Length;
		
		float ballRadius = m_Ball.transform.GetComponent<SphereCollider>().radius;
		
		//print (Globals.g_LevelIDStatic);
		//print (Globals.g_FieldPoints[Globals.g_LevelIDStatic][0].x);
		
		float distance = Globals.g_FieldWidth*(Globals.g_FieldPoints[Globals.g_LevelIDStatic][0].x - Globals.g_FieldPoints[Globals.g_LevelIDStatic][1].x);
		float spacing  = distance/m_BallsToSpawn;
		
		for (int j=0; j < 2; j++)
		{
			for (int i=0; i < m_BallsToSpawn/2; i++)
			{
				//instantiate balls on each side of the field
				GameObject ball = null;
				
				ball = (GameObject) GameObject.Instantiate(m_Ball);
				ball.GetComponent<Ball>().b_IgnoreBump = false;
				ball.transform.position = Globals.g_PlayerInitialPos[j] +
					new Vector3( ((((Globals.g_FieldWidth*Globals.g_FieldPoints[Globals.g_LevelIDStatic][0].x)-(spacing))/2) * i)
						-(Globals.g_FieldWidth*Globals.g_FieldPoints[Globals.g_LevelIDStatic][0].x)+spacing, ballRadius, 0);
			}
		}
		Globals.g_Balls = GameObject.FindGameObjectsWithTag("Ball");
	}
	
	void ResetBalls()
	{
	//	float ballRadius = m_Ball.transform.GetComponent<SphereCollider>().radius;
		
		for (int i = 0; i < Globals.g_TotalBalls; i++)
			Globals.g_Balls[i].GetComponent<Ball>().RestartBall();
	}
	
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Player disconnected: "+player);
		foreach (Player script in m_PlayerScripts)
		{
			if (player == script.GetOwner())
			{//we found the player object of the networkPlayer
				//remove the buffered SetPlayer call
				Network.RemoveRPCs(script.gameObject.networkView.viewID);
				//destroy the playerobject to destroy everything of the disconnected player
				Network.Destroy(script.gameObject);
				//finally remove the script from the list
				m_PlayerScripts.Remove(script);
				break;
			}
		}
		//remove the buffered RCP call for instantiate
		int playerNum = int.Parse(player+"");
		Network.RemoveRPCs(Network.player,playerNum);
		
		//we already destroyed everything since the player never
		//instantiate or buffered RPC
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		
		//update the players in the globals
		Globals.g_Players = GameObject.FindGameObjectsWithTag("Player");
		
		Application.LoadLevel(0);
		
		
	}
	
	void OnNetworkDisconnection(NetworkDisconnection info)
	{
		Debug.Log("Netword disconnected");
		//we reset the entire scene the easy way by reloading it
		//does not work on the clients side, only for server
		Application.LoadLevel(0);
	}
}
