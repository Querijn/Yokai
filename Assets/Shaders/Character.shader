Shader "Flying Penguins/Toon" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecularInAlpha ("Is Specular In Alpha Channel? 1 = Yes 0 = No", Float) = 1
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
		
		_TRIntensity ("Toon ramp intensity", Range(0,5)) = 0.5

		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_AlphaTex ("Alpha map (A)", 2D) = "" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_ToonShade ("Toon Ramp (RGB)", 2D) = "white" { }
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 400
		
		CGPROGRAM
		#pragma surface surf ToonyIGuess alphatest:_Cutoff
		#pragma exclude_renderers flash
		
		sampler2D _MainTex;
		sampler2D _AlphaTex;
		sampler2D _BumpMap;
		sampler2D _ToonShade;
		fixed4 _Color;
		half _Shininess;
		half _TRIntensity;
		half _SpecularInAlpha;
		
		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_AlphaTex;
			float2 uv_BumpMap;
		};

		half4 LightingToonyIGuess(SurfaceOutput s, half3 lightDir, half atten) 
		{
			half NdotL = dot (s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half3 ramp = tex2D (_ToonShade, float2(diff)).rgb;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2) * _TRIntensity;
			c.a = s.Alpha;
			return c;
		}
		
		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 alpha = tex2D(_AlphaTex, IN.uv_AlphaTex);
			
			o.Albedo = tex.rgb*_Color;
			
			if(_SpecularInAlpha<0.5)
			{
				o.Gloss = alpha.a*_Shininess;
				o.Alpha = tex.a;
				o.Specular = _Shininess;
			}
			else
			{
				o.Gloss = tex.a*_Shininess;
				o.Specular = _Shininess;
			}
			
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	FallBack "Transparent/Cutout/VertexLit"
}