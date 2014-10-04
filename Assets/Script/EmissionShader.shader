Shader "Custom/EmissionShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EmissionColor ("Emission Color", Color) = (1.0,1.0,1.0,1.0)
		_Emission ("Emission", Range(0.0,1.0)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _Emission;
		float4 _EmissionColor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Emission = _EmissionColor.rgb * _Emission;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
