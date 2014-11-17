using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSelectHelper : MonoBehaviour 
{
	public List<GameObject> g_PlayerCharacter = new List<GameObject>();
	public List<int> g_PlayerCharacterID = new List<int>();
	public bool g_GlobalsNetworkStatic = false;
	
	void Awake()
	{
		DontDestroyOnLoad(this);
	}
	
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
