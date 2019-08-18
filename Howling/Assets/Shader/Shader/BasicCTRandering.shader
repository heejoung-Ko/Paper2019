Shader "Custom/Toon" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_OutColor("OutLineColor", Color) = (0,0,0,1)
		_ToonFactor("ToonFactor", Range(1, 10)) = 1.0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_OutLineFactor ("OutLine Factor", Range(0, 1)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200




		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf CustomToon fullforwardshadows noambient

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	fixed4 _Color;
	half _ToonFactor;

	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// #pragma instancing_options assumeuniformscaling
	UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutput o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}

	fixed4 LightingCustomToon(SurfaceOutput s, float3 lightDir, float atten)
	{
		float NdotL = dot(s.Normal, lightDir);
		NdotL = max(0, NdotL);
		NdotL = floor(NdotL * _ToonFactor) / (_ToonFactor - 0.5);

		float3 FinalColor = _Color * _LightColor0 * atten * NdotL;
		return fixed4(FinalColor, 1);
	}

	ENDCG





		cull front

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf OutLine fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	fixed4 _Color;
	fixed4 _OutColor;
	half _OutLineFactor;

	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// #pragma instancing_options assumeuniformscaling
	UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v)
	{
		v.vertex.xyz += v.normal.xyz * _OutLineFactor;
		v.normal = -v.normal;

	}

	void surf(Input IN, inout SurfaceOutput o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _OutColor;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}

	fixed4 LightingOutLine(SurfaceOutput s, float3 lightDir, float atten)
	{
		return fixed4(_OutColor.rgb, 1);
	}

	ENDCG

	}
		FallBack "Diffuse"
}
