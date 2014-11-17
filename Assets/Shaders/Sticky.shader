Shader "Flying Penguins/Sticky"
{
    Properties
    {
        //_MainTex("Texture (RGB)", 2D) = "black" {}
        _Color("Color", Color) = (0, 0, 0, 1)
        _OutlineColour("Outline Color", Color) = (0.5, 0.5, 1.0, 1)
        _Size("Size", Float) = 0.3
        _Falloff("Falloff", Float) = 1
        _Transparency("Transparency", Float) = 1
        _Show("Show Outline", Float) = 1
    }
   
	SubShader
    {   
        UsePass "Diffuse/FORWARD"
        Pass
        {
            Name "Outline"
            Tags {"LightMode" = "Always"}
            Cull Front
            ZTest Off
            Blend SrcAlpha One
           
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members worldPos)
#pragma exclude_renderers d3d11 xbox360
                #pragma vertex vert
                #pragma fragment frag
               
                #pragma fragmentoption ARB_fog_exp2
                #pragma fragmentoption ARB_precision_hint_fastest
               
                #include "UnityCG.cginc"
               
                uniform float4 _OutlineColour;
                uniform float _Size;
                uniform float _Falloff;
                uniform float _Show;
                uniform float _Transparency;
               
                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 normal : TEXCOORD0;
                    float3 worldvertpos : TEXCOORD1;
                };

                v2f vert(appdata_base v)
                {
                    v2f o;
                   
                   	if(_Show>0.0001)
                   	{ 
	                    v.vertex.xyz += v.normal*_Size;//(_Size*(1+_SinTime.w))*0.3;
	                    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	                    o.normal = v.normal;
	                    o.worldvertpos = mul(_Object2World, v.vertex);
	                }
                   
                    return o;
                }
              
                float4 frag(v2f i) : COLOR
                {
	                float4 color = _OutlineColour;
                    
                    if(_Show>0.0001)
                   	{
	                    i.normal = normalize(i.normal);
	                    float3 viewdir = normalize(i.worldvertpos-_WorldSpaceCameraPos);
	                    
	                   	if(dot(viewdir, i.normal)>_Size+0.5)//0.5+(0.6*_SinTime.w)*0.3)
	                   	{
	                    	color.a = pow(saturate(dot(viewdir, i.normal)), _Falloff);
	                    	color.a *= _Transparency*_OutlineColour*dot(normalize(i.worldvertpos-_WorldSpaceCameraPos), i.normal);
                    	}
                    }
                    else color.a = 0;
                    
                    return color;
                }
            ENDCG
        }        
    }
   
    FallBack "Diffuse"
}