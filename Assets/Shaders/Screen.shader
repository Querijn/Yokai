Shader "Flying Penguins/Screenspace" 
{
	Properties 
	{
		_MonitorOverlay ("Monitor Overlay", 2D) = "white" {}
		_MainTex ("THIS DOESNT DO ANYTHING DONT CHANGE", 2D) = "white" {}
	}
	 
	SubShader 
	{
	
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
	
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" 
			
			struct v2f 
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
			};
			
			//Our Vertex Shader 
			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				return o; 
			}
			
			sampler2D _MainTex;
			sampler2D _MonitorOverlay;
			
			//Our Fragment Shader
			float4 frag (v2f i) : COLOR
			{

				// CRT monitor
				fixed4 t_RedColour =  tex2D(_MainTex, float2(i.uv.x-0.001, i.uv.y));
				fixed4 t_GreenColour = tex2D(_MainTex, float2(i.uv.x, i.uv.y))*0.8;
				fixed4 t_BlueColour = tex2D(_MainTex, float2(i.uv.x+0.001, i.uv.y));
				return (float4(t_BlueColour.r, t_GreenColour.g, t_RedColour.b, 1.0f)*8)*(tex2D(_MonitorOverlay, i.uv*200)*0.2);

				return tex2D(_MainTex, float2(i.uv.x, i.uv.y)); // Regular
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}