Shader "Flying Penguins/Penis" 
{
	// Spirit shader.
	// Still needs some work, this is the simplest form.
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RimColor ("Rim Color", Color) = (0.0, 0.0, 0.0, 0.0)
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		CULL BACK
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
		
		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		sampler2D _MainTex;
		sampler2D _BumpMap;
		float4 _RimColor;

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo  = c.rgb;
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow (rim, 7.0);
			o.Alpha = rim;          
			o.Gloss = rim;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
