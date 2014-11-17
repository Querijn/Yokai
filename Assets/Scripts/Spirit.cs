using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spirit : MonoBehaviour 
{
	private float m_Deviation;
	private Vector3 m_StartPosition;
	
	private float m_Accumulator = 0.0f;
	private float m_Speed = 1.0f;
	
	void Start () 
	{
		this.transform.LookAt(new Vector3(0,0,0));		
		
		m_Deviation = (float)Random.Range (10.0f, 20.0f);
		m_StartPosition = this.transform.position;
		m_Accumulator = (float)Random.Range(-1.0f,1.0f);
		m_Speed = (float)Random.Range(0.75f,1.0f);
	}
	
	void Update () 
	{
		m_Accumulator += Time.deltaTime*m_Speed;
		
		this.transform.position = m_StartPosition + new Vector3(0,m_Deviation*Mathf.Sin(m_Accumulator),0);
	}
}
