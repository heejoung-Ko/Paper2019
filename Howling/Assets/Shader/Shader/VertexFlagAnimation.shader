﻿Shader "Custom/VertexFlagAnimation" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_FlagFactor("FlagFactor", Range(-0.005, 0.5)) = 0.0
		_FlagSpeed("Flag Speed", Range(0, 30)) = 0.0
		_FlagFrequency("Flag Frequency", Range(0, 30)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		cull off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _FlagFactor;
		half _FlagSpeed;
		half _FlagFrequency;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

			void vert(inout appdata_full v)
		{
			float3 worldPos = unity_ObjectToWorld[3];
			float dist = distance(v.vertex.xz, worldPos.xz);
			v.vertex.x += dist * sin(v.vertex.z * _FlagFrequency + _Time.x * _FlagSpeed) * _FlagFactor;
			v.vertex.y += dist * sin(v.vertex.x * _FlagFrequency + _Time.y * _FlagSpeed) * _FlagFactor;
			v.vertex.z += dist * sin(v.vertex.y * _FlagFrequency + _Time.z * _FlagSpeed) * _FlagFactor;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
