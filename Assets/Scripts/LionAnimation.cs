using UnityEngine;
using System.Collections;

public class LionAnimation : MonoBehaviour 
{
	public string m_AnimationName;
    public Vector3 m_Pos;
    public Vector3 m_Scale;
    public Quaternion m_Orientation;
    bool wasPlaying, m_Repeat = true;
	float m_Delta = 0.0f;

    void Awake()
	{
        m_Pos = transform.position;
		m_Pos.x = -775.2109f;
		m_Pos.y = 411.8155f;
		m_Pos.z = -22.00989f;
        m_Scale = transform.localScale;
        m_Orientation = transform.rotation;
		
		if(m_AnimationName=="")
			m_AnimationName = "Idle";
		
        wasPlaying = false;
    }
	
	public void Flip()
	{
		m_Scale.x = -m_Scale.x;
	}
	
	void Update()
	{
		m_Delta += Time.deltaTime;
		if(m_Delta>15.0f)
		{
			int t_Animation = Random.Range(0,3);
			switch(t_Animation)
			{
			case 0:
				Laugh(); break;
			case 1:
				LookLeft(); break;
			case 2:
				LookRight(); break;
			case 3:
				Idle(); break;
			}
			m_Delta = 0.0f;
		}
	}

    void LateUpdate()
    {
		
		if(m_Repeat==true)
		{
			//print ("Playing new anim "+m_Repeat);
			animation.CrossFade(m_AnimationName);
	    }
		else if(!animation.isPlaying)
		{
			Idle ();
		}
		
		if (!animation.isPlaying && !wasPlaying)
            return; 
		
        transform.localPosition = m_Pos;
        transform.localScale = new Vector3(m_Scale.x, m_Scale.y, m_Scale.z);
        transform.localRotation = m_Orientation;
	}
	
	public void Stop()
	{
		animation.Stop ();
	}
	
	public void StopRepeating()
	{
		m_Repeat = false;
	}
	
	public void Play(string a_Play, float a_Speed = 1.0f, bool a_Repeat = true)
	{
		if(m_AnimationName!=a_Play)
		{
			//print ("Playing new animation: "+a_Play);
			m_Repeat = a_Repeat;
			m_AnimationName = a_Play;
			animation[m_AnimationName].speed = a_Speed;
			animation[m_AnimationName].weight = 0.5f;
			animation.CrossFade(m_AnimationName);
		}
	}
	
	public void ChangeColour(Color newColour)
	{
		renderer.material.color = newColour;
	}
	
	public void LookLeft()
	{
		Play("LookLeft", 1.0f, false); 
	}
	
	public void LookRight()
	{
		Play("LookRight", 1.0f, false); 
	}
	
	public void Laugh()
	{
		Play("Laugh", 1.0f, false); 
	}
	
	public void Idle()
	{
		Play("Idle"); 
	}
}