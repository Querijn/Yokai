Shader "Flying Penguins/Spirit Wall"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader 
	{ 	
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Cull Off
		LOD 200 
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex; 

			float4 frag(v2f_img i) : COLOR 
			{
				float4 t_Colour = tex2D(_MainTex, i.uv);
				clip(t_Colour.a);
				return t_Colour;
			}
			ENDCG
		}
	}
}

//Shader "Flying Penguins/Spirit Wall" 
//{
//	Properties 
//	{
//		_Color ("Main Color", Color) = (1,1,1,1)
//		_Modifier ("Modifier", Range(0,0.010)) = 0.0015
//		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
//	}
//
//	SubShader 
//	{	
//		Pass
//        {
//            Name "Outline"
//            Tags {"LightMode" = "Always"}
//            ZTest Off
//            Blend SrcAlpha One    
//           
//            CGPROGRAM           
//            #pragma fragmentoption ARB_fog_exp2
//            #pragma fragmentoption ARB_precision_hint_fastest
//           
//            #include "UnityCG.cginc"
//           			
//			sampler2D _MainTex;
//			fixed4 _Color;
//			float _Modifier;
//			
//			struct Input 
//			{
//				float2 uv_MainTex;
//			};
//			
//			void frag(Input IN, inout SurfaceOutput o) 
//			{
//				float bloomSize = _Modifier;
//				
//				float4 sum = float4(1.0);
//				
//				//sum += tex2D(_MainTex, float2(IN.uv_MainTex.x - 4*bloomSize, IN.uv_MainTex.y)) * 0.05;
//				//sum += tex2D(_MainTex, float2(IN.uv_MainTex.x - 3*bloomSize, IN.uv_MainTex.y)) * 0.09;
//				//sum += tex2D(_MainTex, float2(IN.uv_MainTex.x - 2*bloomSize, IN.uv_MainTex.y)) * 0.12;
//				//sum += tex2D(_MainTex, float2(IN.uv_MainTex.x -   bloomSize, IN.uv_MainTex.y)) * 0.15;
//				//sum += tex2D(_MainTex, float2(IN.uv_MainTex.x, 				 IN.uv_MainTex.y)) * 0.16;
//				///sum += tex2D(_MainTex, float2(IN.uv_MainTex.x +   bloomSize, IN.uv_MainTex.y)) * 0.15;
//				//sum += tex2D(_MainTex, float2(IN.uv_MainTex.x + 2*bloomSize, IN.uv_MainTex.y)) * 0.12;
//				//sum += tex2D(_MainTex, float2(IN.uv_MainTex.x + 3*bloomSize, IN.uv_MainTex.y)) * 0.09;
//				//sum += tex2D(_MainTex, float2(IN.uv_MainTex.x + 4*bloomSize, IN.uv_MainTex.y)) * 0.05;
//				
//				o.Albedo = sum.rgb;
//				o.Alpha = sum.a;
//			}
//			ENDCG
//        } 
//	}
////	SubShader 
////	{	
////		
////		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
////		Cull Off
////		LOD 200
////	
////		CGPROGRAM
////		#pragma surface surf Lambert alpha
////		
////		sampler2D _MainTex;
////		fixed4 _Color;
////		float _Modifier;
////		
////		struct Input 
////		{
////			float2 uv_MainTex;
////		};
////		
////		void surf (Input IN, inout SurfaceOutput o) 
////		{
////			float bloomSize = _Modifier;
////			
////			float4 sum = float4(0.0);
////			
////			sum += tex2D(_MainTex, float2(IN.uv_MainTex.x - 4*bloomSize, IN.uv_MainTex.y)) * 0.05;
////			sum += tex2D(_MainTex, float2(IN.uv_MainTex.x - 3*bloomSize, IN.uv_MainTex.y)) * 0.09;
////			sum += tex2D(_MainTex, float2(IN.uv_MainTex.x - 2*bloomSize, IN.uv_MainTex.y)) * 0.12;
////			sum += tex2D(_MainTex, float2(IN.uv_MainTex.x -   bloomSize, IN.uv_MainTex.y)) * 0.15;
////			sum += tex2D(_MainTex, float2(IN.uv_MainTex.x, 				 IN.uv_MainTex.y)) * 0.16;
////			sum += tex2D(_MainTex, float2(IN.uv_MainTex.x +   bloomSize, IN.uv_MainTex.y)) * 0.15;
////			sum += tex2D(_MainTex, float2(IN.uv_MainTex.x + 2*bloomSize, IN.uv_MainTex.y)) * 0.12;
////			sum += tex2D(_MainTex, float2(IN.uv_MainTex.x + 3*bloomSize, IN.uv_MainTex.y)) * 0.09;
////			sum += tex2D(_MainTex, float2(IN.uv_MainTex.x + 4*bloomSize, IN.uv_MainTex.y)) * 0.05;
////			
////			o.Albedo = sum.rgb;
////			o.Alpha = sum.a;
////		}
////		ENDCG
////	}
//
//	Fallback "Diffuse"
//}
