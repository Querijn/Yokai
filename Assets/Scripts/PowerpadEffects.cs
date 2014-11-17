using UnityEngine;
using System.Collections;

public class PowerpadEffects : MonoBehaviour 
{
	public GameObject m_PowerupPlane;
	public float m_PlaneYOffset = 30.0f;
	private GameObject m_Plane;
	private bool m_ShowingPlane = false;
	
	public float SinMagnitude = 10.0f;
	public float SinSpeed = 2.0f;
	public float CosMagnitude = 60.0f;
	public float CosSpeed = 3.0f;
	
	private float prevSin = 0.0f;
	private float sinTimer = 0.0f;
	private float cosTimer = 0.0f;
	
	private string m_AnimationName;
    private Vector3 m_Pos;
    private Vector3 m_Scale;
    private Quaternion m_Orientation;
    bool wasPlaying, m_Repeat = true;
	
	private static Texture[] m_Textures = new Texture[9];
	private static Texture[] m_UITextures = new Texture[9];
	
	int m_CurTexture, m_NextTexture;

    void Awake()
	{
        m_Pos = transform.position;
        m_Scale = transform.localScale;
        m_Orientation = transform.rotation;
		
		m_CurTexture = 0;
		m_NextTexture = 0;
		
        wasPlaying = false;
		
		m_Textures[(int)Ball.BallTypes.NORMAL] = (Texture)Resources.Load("Textures/PowerPad/Wood_D", typeof(Texture));// inactive
		m_Textures[(int)Ball.BallTypes.LIGHT] = (Texture)Resources.Load("Textures/PowerPad/WoodLightning_D", typeof(Texture)); 
		m_Textures[(int)Ball.BallTypes.EXPLOSIVE] = (Texture)Resources.Load("Textures/PowerPad/WoodBomb_D", typeof(Texture));
		m_Textures[(int)Ball.BallTypes.CURVE] = (Texture)Resources.Load("Textures/PowerPad/WoodBall_D", typeof(Texture));
		m_Textures[(int)Ball.BallTypes.STICKY] = (Texture)Resources.Load("Textures/PowerPad/WoodSlime_D", typeof(Texture));
		m_Textures[(int)Ball.BallTypes.ROCKET] = (Texture)Resources.Load("Textures/PowerPad/WoodRocket_D", typeof(Texture));
		m_Textures[(int)Ball.BallTypes.HEAVY] = (Texture)Resources.Load("Textures/PowerPad/WoodAnvil_D", typeof(Texture));
		m_Textures[(int)Ball.BallTypes.MAX] = (Texture)Resources.Load("Textures/PowerPad/WoodShield_D", typeof(Texture));
		m_Textures[(int)Ball.BallTypes.MAX + 1] = (Texture)Resources.Load("Textures/PowerPad/woodBooster_D", typeof(Texture));// booster
		
		
		m_UITextures[(int)Ball.BallTypes.NORMAL] = (Texture)Resources.Load("UI/Wood_D", typeof(Texture));// inactive
		m_UITextures[(int)Ball.BallTypes.LIGHT] = (Texture)Resources.Load("UI/light_ball_icon", typeof(Texture)); 
		m_UITextures[(int)Ball.BallTypes.EXPLOSIVE] = (Texture)Resources.Load("UI/explosive_ball_icon", typeof(Texture));
		m_UITextures[(int)Ball.BallTypes.CURVE] = (Texture)Resources.Load("UI/curve_ball_icon", typeof(Texture));
		m_UITextures[(int)Ball.BallTypes.STICKY] = (Texture)Resources.Load("UI/sticky_ball_icon", typeof(Texture));
		m_UITextures[(int)Ball.BallTypes.ROCKET] = (Texture)Resources.Load("UI/rocket_ball_icon", typeof(Texture));
		m_UITextures[(int)Ball.BallTypes.HEAVY] = (Texture)Resources.Load("UI/heavy_ball_icon", typeof(Texture));
		m_UITextures[(int)Ball.BallTypes.MAX] = (Texture)Resources.Load("UI/shield_boost_icon", typeof(Texture));
		m_UITextures[(int)Ball.BallTypes.MAX + 1] = (Texture)Resources.Load("UI/boostIcon", typeof(Texture));// booster
			
//		NORMAL = 0,
//		LIGHT,
//		EXPLOSIVE,
//		CURVE,
//		STICKY,
//		ROCKET,
//		HEAVY
    }
	
	void Update()
	{
		if (m_ShowingPlane && m_Plane != null)
		{
			sinTimer += Time.deltaTime * SinSpeed;
			cosTimer += Time.deltaTime * CosSpeed;
			float sinWave = Mathf.Sin(sinTimer) * SinMagnitude;
			float cosWave = Mathf.Cos(cosTimer) * CosMagnitude;
			
			Vector3 translation = new Vector3(0, sinWave - prevSin, 0);
			
			m_Plane.transform.position += translation;
			
			prevSin = sinWave;
			
			m_Plane.transform.Rotate(Vector3.up,cosWave*Time.deltaTime,Space.World);
		}
	}
	
    void LateUpdate()
    {
        if (!animation.isPlaying && !wasPlaying)
            return; 
		
        transform.localPosition = m_Pos;
        transform.localScale = new Vector3(m_Scale.x, m_Scale.y, m_Scale.z);
        transform.localRotation = m_Orientation;
		
		
		if (this.GetComponent<PowerUpPad>().GetActive())
		{
			if (this.GetComponent<PowerUpPad>().GetType() == 0)// booster
			{
				m_NextTexture = (int)Ball.BallTypes.MAX + 1;
			}
			else// powerup or shield
				m_NextTexture = (int)this.GetComponent<PowerUpPad>().GetPowerup();
			
			//print (this.GetComponent<PowerUpPad>().GetPowerup()+"-"+ m_NextTexture.ToString());
			
			if (!m_ShowingPlane && m_Plane == null)
			{
				m_Plane = (GameObject)Instantiate(m_PowerupPlane,m_Pos+new Vector3(0,m_PlaneYOffset,0),Quaternion.identity);
				
				m_Plane.renderer.material.SetTexture("_MainTex",m_UITextures[m_NextTexture]);
				m_Plane.renderer.material.color = new Color(1.0f,1.0f,1.0f,0.8f);
				m_Plane.transform.LookAt(transform.position + GameObject.FindGameObjectWithTag("MainCamera").transform.rotation * Vector3.down);
				m_ShowingPlane = true;
			}
		}
		else// inactive state
		{
			m_NextTexture = (int)Ball.BallTypes.NORMAL;
			if (m_ShowingPlane && m_Plane != null)
			{
				Destroy(m_Plane);
				m_Plane = null;
				m_ShowingPlane = false;
			}
		}
		
		foreach(Transform t_Child in transform)
		{
			if(t_Child.name.Contains("Mat1"))
				foreach(Transform t_Child2 in t_Child)
					t_Child2.gameObject.renderer.material.SetTexture("_MainTex", m_Textures[m_CurTexture]);
			
			else if(t_Child.name.Contains("Mat2"))
				foreach(Transform t_Child2 in t_Child)
					t_Child2.gameObject.renderer.material.SetTexture("_MainTex", m_Textures[m_NextTexture]);
		}
		
        wasPlaying = animation.isPlaying;
		if(m_Repeat==true && m_AnimationName!="" && m_AnimationName!=null)
		{
			print ("animationPowerPad: "+m_AnimationName);
			animation.CrossFade(m_AnimationName);
		}
    }
	
	public void Stop()
	{
		animation.Stop ();
	}
	
	public void StopRepeating()
	{
		m_Repeat = false;
	}
	
	public void Flip()
	{
		this.Play("Rotate", 1.0f, false);
	}
	
	private void Play(string a_Play, float a_Speed = 1.0f, bool a_Repeat = true)
	{
		if(m_AnimationName!=a_Play||(a_Repeat==false && !animation.isPlaying))
		{
			m_Repeat = a_Repeat;
			m_AnimationName = a_Play;
			animation[m_AnimationName].speed = a_Speed;
			animation[m_AnimationName].weight = 0.5f;
			animation.CrossFade(m_AnimationName);
			
			m_CurTexture = m_NextTexture;
		}
	}
	
	public void ChangeColour(Color newColour)
	{
		renderer.material.color = newColour;
	}
}