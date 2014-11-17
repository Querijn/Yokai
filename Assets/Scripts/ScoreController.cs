using UnityEngine;
using System.Collections;

public class ScoreController : MonoBehaviour 
{
	private int[] m_Score = new int[2];
	
	void Start () 
	{
		Reset();
	}
	
	void Update () 
	{
		/*this.guiText.text = "Score: ";
		
		for(int i = 0; i<m_Score.Length; i++)
		{
			if(i!=0) this.guiText.text += " - ";
			this.guiText.text += ""+m_Score[i];
		}*/
	}
	
	public void Reset()
	{
		for(int i = 0; i<m_Score.Length; i++)
			m_Score[i] = 0;
	}
	
	public int HasPlayerWon()
	{
		for(int i = 0; i<m_Score.Length; i++)
			if(m_Score[i]>=Globals.g_WinningScore)
				return i;
		
		return -1;
	}
	
	public void IncreaseForPlayer(int a_Index, int a_Amount = 1)
	{
		if(a_Index>=0 && a_Index<m_Score.Length)
			m_Score[a_Index]++;
	}
	
	public void DecreaseForPlayer(int a_Index, int a_Amount = 1)
	{
		if(a_Index>=0 && a_Index<m_Score.Length)
			m_Score[a_Index]++;
	}
	
	public void Increase(int a_Index, int a_Amount = 1) { IncreaseForPlayer(a_Index, a_Amount); }
	public void Decrease(int a_Index, int a_Amount = 1) { DecreaseForPlayer(a_Index, a_Amount); }
	
	public int GetScore(int a_Index)
	{
		if(a_Index>=0 && a_Index<m_Score.Length)
			return m_Score[a_Index];
		return 0;
	}
}
