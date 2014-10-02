Shader "Tango/ColorDepthBlend" {
	Properties 
	{
	      _ColorTex ("color Texture", 2D) = "white" {}
	      _DepthTex ("Depth Texture", 2D) = "white" {}
	      _BlendValue ("Blend Value", Float) = 0.0
	}
	SubShader 
	{
		Pass 
		{
			GLSLPROGRAM
			
			uniform sampler2D _ColorTex;
			uniform sampler2D _DepthTex;	
     		uniform float _BlendValue;
     		
         	varying vec4 textureCoordinates; 
			
			#ifdef VERTEX
			void main()
			{
				textureCoordinates = gl_MultiTexCoord0;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
			
			#endif

			#ifdef FRAGMENT
			void main()
			{
				float alpha = clamp(_BlendValue, 0.0, 1.0);
				// the color image come fliped in Y axis
				vec4 t0 = texture2D(_ColorTex, vec2(textureCoordinates.s, 1.0-textureCoordinates.t));
			    vec4 t1 = texture2D(_DepthTex, textureCoordinates.st);
			    gl_FragColor = (1.0-alpha) * t0 + alpha * t1;
			}
			#endif

			ENDGLSL
		}
	}
}

