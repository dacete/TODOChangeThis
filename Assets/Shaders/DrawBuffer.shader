Shader "Custom/DrawBuffer"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="MarchingCubes" }
		
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 5.0
		#include "UnityCG.cginc"
		struct Vertex
		{
			float4 position;
			float3 normal;
			float dummy;
		};
		struct appdata_custom{
		uint i : SV_VertexID;
			         float4 vertex    : POSITION;  // The vertex position in model space.
     float3 normal    : NORMAL;    // The vertex normal in model space.
     float4 texcoord  : TEXCOORD0; // The first UV coordinate.
     float4 texcoord1 : TEXCOORD1; // The second UV coordinate.
     float4 tangent   : TANGENT;   // The tangent vector in Model Space (used for normal mapping).
     float4 color     : COLOR;   
		};
		half4 _Color;
		half _Glossiness;
		half _Metallic;

#ifdef SHADER_API_D3D11
	StructuredBuffer<Vertex> _Buffer;
	float4x4 objMat;
#endif


void GetVertexData(in uint id,
		inout float4 position,
		inout half3 normal,
		inout float2 uv0,
		inout float2 uv1)
	{
#ifdef SHADER_API_D3D11
		Vertex vert = _Buffer[id];
		position = vert.position;
		normal = vert.normal;

		position = mul(objMat, float4(position.xyz, 1));
		normal = normalize(mul((float3x3)objMat, normal));
#endif
	}


	struct Input
		{
			float not_in_use;
		};


	//ddd
		void vert(inout appdata_custom v)
		{
		float2 uv0;
		uv0 = float2(v.texcoord.x,v.texcoord.y);
		float2 uv1;
		uv1 = float2(v.texcoord1.x,v.texcoord1.y);
			GetVertexData(v.i,v.vertex,v.normal,uv0,uv1);
		}

		

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Color.a;
			

		}
		ENDCG
	} 
	FallBack "Diffuse"
}
