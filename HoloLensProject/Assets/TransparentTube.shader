Shader "Custom/TransparentTube" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Min("Min", Vector) = (0,0,0,0)
		_Max("Max", Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		//Cull OFF

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float3 worldPos;	
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _Min;
		fixed4 _Max;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)


		//Tube Transparency is calculated here
		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color;
			o.Alpha = 1;
			if (IN.worldPos.x > _Min[0] && IN.worldPos.x < _Max[0] &&
				IN.worldPos.y > _Min[1] && IN.worldPos.y < _Max[1] &&
				IN.worldPos.z > _Min[2] && IN.worldPos.z < _Max[2])
				o.Alpha = _Color.a;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
