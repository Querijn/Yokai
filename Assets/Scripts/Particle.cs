using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		/*if(gameObject.networkView==null)
		{
			gameObject.AddComponent(typeof(NetworkView));
			gameObject.networkView.observed = transform;
			gameObject.networkView.stateSynchronization = NetworkStateSynchronization.Off;
	
		}*/
	}
	
	private Vector3 t_V = new Vector3(0,0,0);
	
	// Update is called once per frame
	void Update () 
	{

		if(this.gameObject.particleSystem.isStopped)
			Destroy(this.gameObject);

		
		UnityEngine.ParticleSystem.Particle[] allParticles = new UnityEngine.ParticleSystem.Particle[particleSystem.particleCount];
		particleSystem.GetParticles(allParticles);
		 
		for(int p = 0; p<allParticles.Length; p++)
		{
		    if(allParticles[p].position.y<0.0f)
			{
				t_V = allParticles[p].position;
				t_V.y = 0.0f;
				allParticles[p].position = t_V;
				
				t_V = allParticles[p].velocity;
				t_V *= 0.8f;
				t_V.y *= -1.0f;
				allParticles[p].velocity = t_V;
			}
		}
		particleSystem.SetParticles(allParticles, allParticles.Length);
		// Can't control individual particles ;-;
	}
}