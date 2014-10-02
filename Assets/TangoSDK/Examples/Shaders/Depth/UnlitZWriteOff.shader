Shader "Tango/Unlit/UnlitZWriteOff" {
	Properties 
	{	
		_MainColor ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Depth Texture", 2D) = "white" {}
	}
	SubShader 
	{
		ZWrite Off
		Pass 
		{
			GLSLPROGRAM
			#include "UnityCG.glslinc"
			uniform lowp vec4 _MainColor;
			uniform lowp sampler2D _MainTex;
			uniform lowp vec4 _MainTex_ST;
			varying lowp vec2 uv; 
			#ifdef VERTEX
			
			void main()
			{
				uv = (gl_MultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
				
			}
			#endif
 
			#ifdef FRAGMENT
			void main()
			{
				gl_FragColor = texture2D(_MainTex, uv) * _MainColor;
			}
			
			#endif
			ENDGLSL
		}
	}
}