Shader "Flying Penguins/Correct AlphaBumpedSpecular" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Transparency (A)", 2D) = "white" {}
		_AlphaTex ("Alpha map (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 400
		
		CGPROGRAM
		#pragma surface surf WrapLambert alphatest:_Cutoff
		#pragma exclude_renderers flash
		
		sampler2D _MainTex;
		sampler2D _AlphaTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		half _Shininess;

		half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) 
		{
			half NdotL = dot (s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
			c.a = s.Alpha;
			return c;
		}
		
		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_AlphaTex;
			float2 uv_BumpMap;
		};
		
		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 alpha = tex2D(_AlphaTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = alpha.a * _Color.a;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	FallBack "Transparent/Cutout/VertexLit"
}