using UnityEngine;
using System.Collections;

public class SimplePropAnimation : MonoBehaviour 
{
	public string m_AnimationName;
    public Vector3 m_Pos;
    public Vector3 m_Scale;
    public Quaternion m_Orientation;
    bool wasPlaying, m_Repeat = true;
	public float m_Delta = 0;
	
	public bool m_RandomStart = false;

    void Awake()
	{
        m_Pos = transform.position;
        m_Scale = transform.localScale;
        m_Orientation = transform.rotation;
		
		if(m_AnimationName=="")
			m_AnimationName = "Neutral";
		
        wasPlaying = false;
		
		if(m_RandomStart) animation[m_AnimationName].time = Random.Range(0.0f, animation[m_AnimationName].length);
    }
	
	public void Flip()
	{
		m_Scale.x = -m_Scale.x;
	}

    void LateUpdate()
    {
        if (!animation.isPlaying && !wasPlaying)
            return; 
		
        transform.localPosition = m_Pos;
		transform.localScale = m_Scale;
        transform.localRotation = m_Orientation;

        wasPlaying = animation.isPlaying;
		if(m_Repeat==true)
			animation.CrossFade(m_AnimationName);
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
			m_Repeat = a_Repeat;
			m_AnimationName = a_Play;
			if(m_RandomStart) animation[m_AnimationName].time = Random.Range(0.0f, animation[m_AnimationName].length);
			animation[m_AnimationName].speed = a_Speed;
			animation[m_AnimationName].weight = 0.5f;
			animation.CrossFade(m_AnimationName);
			//print (m_AnimationName);
		}
	}
	
	public void ChangeColour(Color newColour)
	{
		renderer.material.color = newColour;
	}
}