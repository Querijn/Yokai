using UnityEngine;
using System.Collections;

public class UIScript : MonoBehaviour 
{

	private static Texture[] m_Textures = new Texture[4];
	
	public int assetNum = 0; //change this in inspector for each different UI piece, i.e. player0/player1 powerup background and timer background
	// Use this for initialization
	void Start () 
	{
		m_Textures[(int)0] = (Texture)Resources.Load("UI/UI_Timer_Chaos", typeof(Texture));
		m_Textures[(int)1] = (Texture)Resources.Load("UI/UI_Dragon_Chaos", typeof(Texture)); //change this
		m_Textures[(int)2] = (Texture)Resources.Load("UI/UI_Dragon_Chaos_Flipped", typeof(Texture)); //change this
		m_Textures[(int)3] = (Texture)Resources.Load("UI/UI_ChaosTimer", typeof(Texture)); //change this

		guiTexture.texture = m_Textures[assetNum];
	}
	
	// Update is called once per frame
	void Update () 
	{
		guiTexture.enabled = true;
	}
}
