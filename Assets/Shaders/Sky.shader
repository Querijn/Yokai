Shader "Flying Penguins/Sky" 
{
	Properties 
	{
		g_Color ("Main Color", Color) = (1,1,1,1)
        g_Noise("Noise (RGB)", 2D) = "white" {}
		g_Effect ("Effect", Range(0,1)) = 0
		g_Border("Border Range", Range(0,1)) = 0.8
		g_WhiteRange("White Range", Range(0,12)) = 9.6
		g_Direction ("Direction", Range(0,360)) = 0
		g_Tex ("Sky Cubemap", Cube) = "white" {}
	}
	
	
	SubShader 
	{
		Tags { "Queue"="Background" "RenderType"="Background" }

		ZWrite Off 
		Cull Off 
		
		CGPROGRAM
		#pragma surface SurfaceShader Lambert
		#pragma target 3.0
		// Because I am a bad programmer

		fixed4 g_Color;
		sampler2D g_Noise;
		float g_Border;
		float g_WhiteRange;
		float g_Direction;
		float g_Effect;
		samplerCUBE g_Tex;

		struct Input 
		{
			float4 screenPos;
			float2 uvg_Noise;
            float3 worldRefl;
		};
	
		void SurfaceShader(Input IN, inout SurfaceOutput o) 
		{
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
				
			float t_Angle = 0.0174532925*g_Direction;
			
			float t_Cos = cos(t_Angle);
			float t_Sin = sin(t_Angle);
			
			screenUV.x -= 0.5;
			screenUV.y -= 0.5;
			
			screenUV.x = screenUV.x * t_Cos - screenUV.y * t_Sin;
			screenUV.y = screenUV.x * t_Sin + screenUV.y * t_Cos;
			
			screenUV.x += 0.5;
			screenUV.y += 0.5;
			
			fixed4 t_TextureColour = tex2D(g_Noise, float2(screenUV.x*2, screenUV.y*0.5+_Time*125));
			o.Emission = (1-g_Effect)*texCUBE(g_Tex, IN.worldRefl).rgb; // We want less of the skybox if we're showing the effect.
			
			if(g_Effect>0.01)
			{
				// The gradient:
				fixed4 t_Colour = fixed4(0.0);
				
				if(screenUV.x<0.5) 
					t_Colour = g_Color*(screenUV.x)*2;
				else t_Colour = g_Color*(1-(screenUV.x))*2;
				
				// Small colour adjustment:
				if(t_Colour.b>g_Border)
				{
					t_Colour.r += (t_Colour.b-g_Border)*g_WhiteRange;
					t_Colour.g += (t_Colour.b-g_Border)*g_WhiteRange;
				}
				
				// The stripes:
				//fixed4 t_TextureColour = tex2D(g_Noise, float2(screenUV.x*2, screenUV.y*0.5+_Time*125));
				
				if(t_TextureColour.r>0.1)
					o.Albedo += g_Effect*t_Colour.rgb * (1.+t_TextureColour);
				else o.Albedo += g_Effect*t_Colour.rgb; // We want more of this effect when g_Effect hits 1
				//o.Emission = fixed(0.0);
			}
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
