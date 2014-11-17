using UnityEngine;
using System.Collections;

public class PowerUpManager : MonoBehaviour 
{
	//Array of all the powerpads
	private GameObject[] m_PowerPads;
	
	private int m_NumPads = 0;
	
	private int m_CurrentActive = 0;
	
	public int m_PowerUpPadInitialDelay = 8;
	public int m_PowerUpPadMinimumDelay = 5;
	public int m_PowerUpPadMaximumDelay = 9;
	
	public bool g_Active = false;
	// Use this for initialization
	void Start () 
	{
		m_PowerPads = GameObject.FindGameObjectsWithTag("PowerPad");
		
		m_NumPads = m_PowerPads.Length;
		print (m_NumPads);
		DisactiveAll();
	}
	
	// Update is called once per frame
	void Update () 
	{
		int num = 0;
		for(int i = 0; i < m_NumPads; i++)
		{
			PowerUpPad powerPad = m_PowerPads[i].GetComponent<PowerUpPad>();
			
			if(powerPad.GetActive() == true)
			{
				num++;
				//DisactiveAll(i);
			}
		}
		
		m_CurrentActive = num;
		
	}
	
	public void ActivateRandom()
	{
		if(m_CurrentActive <= 0)
		{
			int activeThis = Random.Range(0, m_NumPads);
			
			//print ("ACTIVATE THIS PAD: " + activeThis);
		
			DisactiveAll();
			m_PowerPads[activeThis].GetComponent<PowerUpPad>().Activate();
			DisactiveAll(activeThis);
		}
		
		g_Active = true;
	}
	
	public void DisactiveAll(int except)
	{
		for(int i = 0; i < m_NumPads; i++)
		{	
			if(i != except)
			{
				PowerUpPad powerPad = m_PowerPads[i].GetComponent<PowerUpPad>();
				//powerPad.SetActive(false);
				powerPad.Reset();
				
			}
		}
	
	}
	
	void DisactiveAll()
	{
		for(int i = 0; i < m_NumPads; i++)
		{	
			PowerUpPad powerPad = m_PowerPads[i].GetComponent<PowerUpPad>();
			//powerPad.SetActive(false);
			powerPad.Reset();
		}
	
	}
}
