using UnityEngine;
using System.Collections;

public class Connect: MonoBehaviour 
{
	private string m_ConnectToIP = "127.0.0.1";
	private int m_ConnectPort = 25000;
	
	void OnGUI()
	{
		if (Globals.g_IsNetworkStatic == true)
		{
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				
				GUILayout.BeginArea(new Rect(Screen.width/2 - 128, Screen.height/2 - 128, 256,256));
			
				//We are not connected yet
				GUILayout.Label("Connection status: Disconnected");
			
				m_ConnectToIP = GUILayout.TextField(m_ConnectToIP, GUILayout.MinWidth(100));
				m_ConnectPort = int.Parse(GUILayout.TextField(m_ConnectPort.ToString()));
				GUILayout.BeginVertical();
				
				if (GUILayout.Button ("Connect as client"))
				{
					//Connect to the "connectToIP" and "connectPort" as entered via the GUI
					Network.useNat = false;
					Network.Connect(m_ConnectToIP, m_ConnectPort);
				}
				
				if (GUILayout.Button ("Start Server"))
				{
					//Start a server for 2 clients using the "connectPort" given via the GUI
					Network.useNat = false;
					Network.InitializeServer(1, m_ConnectPort);
				}
				GUILayout.EndVertical();
				
				GUILayout.EndArea();
			}
			else
			{
				GUILayout.BeginArea(new Rect(Screen.width-256, 0, 256,256));
				if (Network.peerType == NetworkPeerType.Connecting)
				{
					GUILayout.Label("Connection status: Connecting");
				}else if (Network.peerType == NetworkPeerType.Client)
				{
					GUILayout.Label("Connection status: Client!");
					GUILayout.Label("Ping to server: "+Network.GetAveragePing(Network.connections[0]));		
				}
				else if (Network.peerType == NetworkPeerType.Server)
				{
					GUILayout.Label("Connection status: Server!");
					GUILayout.Label("Connections: "+Network.connections.Length);
					GUILayout.Label("IP address: "+Network.player.ipAddress);
					
					if(Network.connections.Length>=1)
					{
						GUILayout.Label("Ping to first player: "+Network.GetAveragePing(  Network.connections[0] ) );
					}			
				}

				if (GUILayout.Button ("Disconnect"))
				{
					Network.Disconnect(200);
				}
				GUILayout.EndArea();
			}
		}
		
	}
}
