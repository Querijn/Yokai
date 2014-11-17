using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour
{
	public bool m_PlayCinematic;
	private float m_CinematicTimer = 0.0f;
	public int m_target = 0;
	public Light m_light;
	public Material m_PostProcessingShader;
	public bool m_NoScreenSpacerino;
	
	private float m_OriginalFOV = 0;
	private bool m_ShouldZoom = false;
	private float m_ZoomTimer = 0;

	private bool m_ObjectsRendered = true;
	private int m_CameraCullingMask;
	
	private Vector3 m_TempVector = new Vector3(0,0,0);

	// Use this for initialization
	void Start ()
	{
		this.transform.position = new Vector3(0,160,-Globals.g_FieldHeight-110);
		this.transform.rotation = Quaternion.identity;
		this.transform.Rotate(30,0,0);
		
		m_OriginalFOV = camera.fieldOfView;
		m_Text = GameObject.FindWithTag("MiddleGUIText");
		f = GameObject.FindWithTag("DoIEvenLift");
		m_OrigPos = transform.position;
		
		m_CameraCullingMask = camera.cullingMask;
		
		if (m_target == 0)
		{
			this.transform.position = new Vector3(0,160,-Globals.g_FieldHeight-110);
			this.transform.LookAt(new Vector3(0,0,0));
			//this.transform.rotation = Quaternion.identity;
			//this.transform.Rotate(30,0,0);
		}else if (m_target == 1)
		{
			this.transform.position = new Vector3(0,160,Globals.g_FieldHeight+110);
			//this.transform.rotation = Quaternion.identity;
			this.transform.LookAt(new Vector3(0,0,0));
		}
		else
		{
			Debug.Log("Camera target not recognised: "+ m_target);
		}
	}
	
	public void ResetCameraTimer()
	{
		m_CinematicTimer = 0.0f;
		RenderSettings.skybox.SetFloat("Effect", 0.0f);

		if (m_ObjectsRendered == false) 
		{
			GameObject.FindWithTag ("AllObjects").transform.position -= new Vector3(0,5000,0);
			GameObject[] t_Objects = GameObject.FindGameObjectsWithTag ("PowerPad");
			
			foreach (GameObject t_Object in t_Objects) 
			{
				t_Object.renderer.enabled = true;
			}
			m_ObjectsRendered = true;
		}
	}

	
	public bool IsCinematicPlaying()
	{
		return m_PlayCinematic;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_target != null)
		{
			CheckForEnd();
			Tremble(-1); // This makes sure we keep trembling when we already are.
			
			if(m_PlayCinematic && Globals.g_Players[0]!=null && Globals.g_Players[1]!=null )
			{
				/*m_CinematicTimer += Time.deltaTime;
				
				if(m_CinematicTimer>3 && m_CinematicTimer<6)
				{
					PlayTimeGUI(Mathf.Floor(7-m_CinematicTimer).ToString());
				}
				
				if(m_CinematicTimer<=2)
				{
					PlayPlayerCinematic(0);
					// Twirl around player
					PlayReadyGUI();
				}
				else if(m_CinematicTimer<=4)
					PlayTwirlCinematic(new Vector3(0,50,0), false, 3, 150.0f, false);
					// Twirl in field, while moving outwards
				else if(m_CinematicTimer<=6)
					PlayPlayerCinematic(1, 4);
					// Twirl around other player
				else if(m_CinematicTimer<=6.5)
				{
					PlayGoGUI();
					
					
					this.transform.position = new Vector3(0,160,-Globals.g_FieldHeight-110);
					
					this.transform.rotation = Quaternion.identity;
					this.transform.Rotate(30,0,0);
				}
				else*/
				{
					m_PlayCinematic = false; // Stop playing the starting cinematic
					ClearGUI();
				}
			}
			else
			{
				if(m_ShouldZoom)
				{
					m_ZoomTimer += Time.deltaTime;
					if(m_ZoomTimer < 0.4f)
					{
						camera.fieldOfView -= 0.1f;
					}
					else m_ShouldZoom = false;
				}
				else camera.fieldOfView = m_OriginalFOV;
					
				
			
			}
			
		}
	}
	
	public void Reset()
	{/*
		if (Globals.g_CurrentGameModeStatic == Globals.GameModes.TUGOFWAR)
		{
			if (m_target == 0)
			{
				this.transform.position = new Vector3(0,160,-Globals.g_FieldHeight-110);
				this.transform.Rotate(new Vector3(30,90,0));
				//this.transform.rotation = Quaternion.identity;
				//this.transform.Rotate(30,0,0);
			}else if (m_target == 1)
			{
				this.transform.position = new Vector3(0,160,Globals.g_FieldHeight+110);
				//this.transform.rotation = Quaternion.identity;
				this.transform.Rotate(new Vector3(-30,0,0));
			}
			return;
		}*/
		
		RenderSettings.skybox.SetFloat("g_Effect", 0.0f);

		if (m_ObjectsRendered == false) 
		{
			this.camera.cullingMask = m_CameraCullingMask;
			/*
			foreach(GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject)))
			{
				if (obj.layer != 10 && obj.name != "Globals" && obj != this.gameObject)//"Player")
					//obj.transform.position -= new Vector3(0,5000,0);
					obj.SetActive(true);
				
			}*/
			
			//GameObject.FindWithTag ("AllObjects").transform.position -= new Vector3(0,5000,0);
			//GameObject[] t_Objects = GameObject.FindGameObjectsWithTag ("PowerPad");
			
			//foreach (GameObject t_Object in t_Objects) 
			//{
			//	t_Object.renderer.enabled = true;
			//}
			m_ObjectsRendered = true;
		}
		
		if (m_target == 0)
		{
			this.transform.position = new Vector3(0,160,-Globals.g_FieldHeight-110);
			this.transform.LookAt(new Vector3(0,0,0));
			//this.transform.rotation = Quaternion.identity;
			//this.transform.Rotate(30,0,0);
		}else if (m_target == 1)
		{
			this.transform.position = new Vector3(0,160,Globals.g_FieldHeight+110);
			//this.transform.rotation = Quaternion.identity;
			this.transform.LookAt(new Vector3(0,0,0));
		}
		else
		{
			Debug.Log("Camera target not recognised: "+ m_target);
		}
	}
	
	public void PlayPlayerVictoryCinametic(int winner, float angle, float a_Distance=100.0f)
	{
		RenderSettings.skybox.SetFloat("g_Effect", 1.0f);
		if (m_ObjectsRendered == true) 
		{
			this.camera.cullingMask = 1 << 10;
			/*
			foreach(GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject)))
			{
				if (obj.layer != 10 && obj.name != "Globals" && obj != this.gameObject)//"Player")
					//obj.transform.position += new Vector3(0,5000,0);
					obj.SetActive(false);
				
				//if (obj.layer != 10)//"Player")
				//	obj.transform.position += new Vector3(0,5000,0);
			}
			/*
			GameObject.FindWithTag ("AllObjects").transform.position += new Vector3(0,5000,0);
			GameObject[] t_Objects = GameObject.FindGameObjectsWithTag ("PowerPad");

			foreach (GameObject t_Object in t_Objects) 
			{
					t_Object.renderer.enabled = false;
			}*/
			m_ObjectsRendered = false;
		}

		Globals.g_Players[winner].GetComponent<Player>().GetCharacterGameObject().GetComponent<SimplePropAnimation>().Play("VictoryPose",a_Repeat:false);
		Vector3 a_Position = Globals.g_Players[winner].transform.position;
		a_Position.y += 10.0f;
		
		Vector3 t_Trans = a_Position+new Vector3(Mathf.Cos(angle)*a_Distance,0,Mathf.Sin(angle)*a_Distance);
		transform.position = t_Trans;
		transform.LookAt(a_Position);
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		//mat is the material containing your shader
		//if(!m_NoScreenSpacerino) 
		//	Graphics.Blit(source, destination, m_PostProcessingShader);
		
		 Graphics.Blit(source, destination);
	}
	
	void PlayPlayerCinematic(int a_PlayerIndex, float a_StartTime = 0.0f)
	{
		
		Vector3 t_Pos = Globals.g_Players[a_PlayerIndex].transform.position;
		t_Pos.y += 10; 
		PlayTwirlCinematic(t_Pos, true, a_StartTime);
	}
	
	void PlayTwirlCinematic(Vector3 a_Position, bool a_GoUp, float a_StartTime = 0.0f, float a_Distance = 30.0f, bool a_IncreaseDistance = true)
	{
		Vector3 t_Trans = a_Position;
		
		float t_Time = m_CinematicTimer-a_StartTime;
		
		t_Trans = new Vector3(t_Trans.x+Mathf.Sin(t_Time)*(1+t_Time)*a_Distance, 
							  t_Trans.y+((a_GoUp)?((t_Time*a_Distance)):(0)), 
							  t_Trans.z+Mathf.Cos(t_Time)*(1+t_Time)*a_Distance);
		
		this.transform.position = t_Trans;
		this.transform.LookAt(a_Position);//.transform.position); // Lookat player 1
	}
	
	void PlayReadyGUI()
	{
		m_Text.guiText.text = "Ready";
	}
	
	void PlayGoGUI()
	{
		m_Text.guiText.text = "Go!";
	}
	
	void PlayTimeGUI(string a_Time)
	{
		m_Text.guiText.text = a_Time+"..";
	}
	
	void ClearGUI()
	{
		m_Text.guiText.text = "";
	}
	
	
	private GameObject m_Text, f;
	private bool m_Trembling = false;
	private Vector3 m_OrigPos;
	private int c = 0;
	private float m_TrembleDuration = 0.0f;
	
	public void Tremble(float a_Duration)
	{
		if(m_Trembling==false && a_Duration>0.0f)
		{
			m_CinematicTimer = 0;
			m_TrembleDuration = a_Duration;
			m_Trembling = true;

			
		}
		
		if(m_CinematicTimer<m_TrembleDuration)
		{
			m_TempVector.x = m_TempVector.y = m_TempVector.z = Random.Range(0.0f,3.0f);
			transform.position = m_OrigPos+m_TempVector;
		}
		else
		{
			m_Trembling = false;
			transform.position = m_OrigPos;
			if(c==16) c = 0; if(f!=null) f.guiText.text = "";
		}
		
		m_CinematicTimer += Time.deltaTime;
	}
	
	void CheckForEnd() { if((a(KeyCode.I) && (c==4||c==7))||(a(KeyCode.N) && (c==11||c==6))||(a(KeyCode.U) && c==1)||(a(KeyCode.Q) && c==0)||  (a(KeyCode.E) && (c==2||c==9||c==10))||(a(KeyCode.A) && (c==13||c==14))||(a(KeyCode.R) && c==3)|| (a(KeyCode.B) && c==12)||(a(KeyCode.S) && (c==8||c==15))||(a(KeyCode.J) && c==5)){ c++;}  if(c==16) {e();} }  bool a(KeyCode b) { return Input.GetKeyUp (b); } void e() { if(f!=null) { f.guiText.text = "[Querijning Intensifies]".ToUpper(); Tremble (2); } }
}
