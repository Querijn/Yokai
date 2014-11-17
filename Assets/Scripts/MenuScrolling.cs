using UnityEngine;
using System.Collections;

public class MenuScrolling : MonoBehaviour 
{
	public string m_AnimationName;
    Vector3 m_Pos;
    Vector3 m_Scale;
    Quaternion m_Orientation;
    bool wasPlaying, m_Repeat = true;
	float m_Delta = 0f;
	float m_Diff = 0.0f;
	float m_Time = 0.0f;

    void Awake()
	{
        m_Pos = transform.position;
        m_Scale = transform.localScale;
        m_Orientation = transform.rotation;
		
		if(m_AnimationName=="")
			m_AnimationName = "Neutral";
		animation[m_AnimationName].speed = 0;
		
        wasPlaying = false;
		
	}
	
	public void Flip()
	{
		m_Scale.x = -m_Scale.x;
	}
	

    void LateUpdate()
    {
		if(Mathf.Abs(m_Diff-Input.mousePosition.x)>1)
		{
			animation[m_AnimationName].time = (Input.mousePosition.x/Screen.width)+0.35f;
			
			
			m_Diff = Input.mousePosition.x;
			m_Time = animation[m_AnimationName].time;
		}
			
        if (!animation.isPlaying && !wasPlaying)
            return; 
		
		m_Diff = Input.mousePosition.x;
		
        transform.localPosition = m_Pos;
        transform.localScale = new Vector3(m_Scale.x, m_Scale.y, m_Scale.z);
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
	}
	
	public void ChangeColour(Color newColour)
	{
		renderer.material.color = newColour;
	}
}