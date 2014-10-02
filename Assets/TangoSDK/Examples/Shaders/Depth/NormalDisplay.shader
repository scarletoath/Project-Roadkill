Shader "Tango/NormalDisplay" {


	Properties 
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0.0, 1.0, 0.0, 1.0)
	}
	
	SubShader {
		
		
		Pass {  
			GLSLPROGRAM
			uniform sampler2D _MainTex;
			uniform vec4 _Color;
			varying vec3 n;
			varying vec4 textureCoordinates; 
			#ifdef VERTEX

			void main()
			{                                
				n = normalize(gl_Normal);
				textureCoordinates = gl_MultiTexCoord0;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}

			#endif

			#ifdef FRAGMENT

			void main()
			{
				gl_FragColor = vec4(n.x, n.y, n.z, 1.0);
			}
			#endif

			ENDGLSL
		}
	} 

}