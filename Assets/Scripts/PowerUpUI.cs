using UnityEngine;
using System.Collections;

public class PowerUpUI : MonoBehaviour {

	private static Texture[] m_Textures = new Texture[7];
	
	public int playerID = 0;
	// Use this for initialization
	void Start () 
	{
		//		LIGHT = 0,
//		NORMAL,
//		EXPLOSIVE,
//		CURVE,
//		STICKY,
//		ROCKET,
//		HEAVY
		m_Textures[(int)0] = (Texture)Resources.Load("UI/wood_D", typeof(Texture));
		m_Textures[(int)1] = (Texture)Resources.Load("UI/light_ball_icon", typeof(Texture));
		m_Textures[(int)2] = (Texture)Resources.Load("UI/explosive_ball_icon", typeof(Texture));
		m_Textures[(int)3] = (Texture)Resources.Load("UI/curve_ball_icon", typeof(Texture));
		m_Textures[(int)4] = (Texture)Resources.Load("UI/sticky_ball_icon", typeof(Texture));
		m_Textures[(int)5] = (Texture)Resources.Load("UI/rocket_ball_icon", typeof(Texture));
		m_Textures[(int)6] = (Texture)Resources.Load("UI/heavy_ball_icon", typeof(Texture));
		
		
		float scaleFactor = (float)Screen.height/750;
		
		
		
		int texWidth = (int)(guiTexture.pixelInset.width * scaleFactor);
		int texHeight = (int)(guiTexture.pixelInset.height * scaleFactor);
		
		print ("height: " + Screen.height);
	
		int powerupPositionOffsetY = 8;//5;
		int powerupPositionOffsetX = 20;//7;
		
		if(playerID == 0)
		{
			//bottom
			guiTexture.pixelInset = new Rect(-texWidth-powerupPositionOffsetX,(powerupPositionOffsetY+4)*scaleFactor,texWidth,texHeight);
		}
		else
		{
			guiTexture.pixelInset = new Rect(-(Screen.width)+powerupPositionOffsetX,Screen.height-texHeight-powerupPositionOffsetY*scaleFactor,texWidth,texHeight );
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		int token = Globals.g_Players[playerID].GetComponent<Player>().GetSpecialToken();
		this.guiTexture.enabled = false;
		//print();
		if(token!=-1)
		{
			this.guiTexture.texture = m_Textures[token];
			
			
			if((Ball.BallTypes)token!=Ball.BallTypes.NORMAL)
				this.guiTexture.enabled = true;
		}
		
		
	}
}
