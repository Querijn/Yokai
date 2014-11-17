using UnityEngine;
using System.Collections;

public class ScreenSpacerinoClone : MonoBehaviour
{
	public Material m_PostProcessingShader;
	public bool m_NoScreenSpacerino;
	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		//mat is the material containing your shader
		if(!m_NoScreenSpacerino) Graphics.Blit(source, destination, m_PostProcessingShader);
		else Graphics.Blit(source, destination);
	}	
}
