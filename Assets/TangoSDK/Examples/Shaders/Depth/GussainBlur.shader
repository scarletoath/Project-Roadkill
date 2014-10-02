Shader "Tango/GussainBlur" {
	Properties 
	{	
		_DepthTex ("Depth Texture", 2D) = "white" {}
		_Filter("Filter", int) = 0
	}
	SubShader 
	{
		Pass 
		{
			GLSLPROGRAM
			#include "UnityCG.glslinc"
			uniform sampler2D _DepthTex;	
         	uniform int _Filter;
         	
         	varying vec4 textureCoordinates; 
			varying vec2 v_blurTexCoords[14];
			#ifdef VERTEX
			
			void main()
			{
				textureCoordinates = gl_MultiTexCoord0;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
				
				v_blurTexCoords[ 0] = textureCoordinates.st + vec2(-0.028, 0.0);
				v_blurTexCoords[ 1] = textureCoordinates.st + vec2(-0.024, 0.0);
				v_blurTexCoords[ 2] = textureCoordinates.st + vec2(-0.020, 0.0);
				v_blurTexCoords[ 3] = textureCoordinates.st + vec2(-0.016, 0.0);
				v_blurTexCoords[ 4] = textureCoordinates.st + vec2(-0.012, 0.0);
				v_blurTexCoords[ 5] = textureCoordinates.st + vec2(-0.008, 0.0);
				v_blurTexCoords[ 6] = textureCoordinates.st + vec2(-0.004, 0.0);
				v_blurTexCoords[ 7] = textureCoordinates.st + vec2( 0.004, 0.0);
				v_blurTexCoords[ 8] = textureCoordinates.st + vec2( 0.008, 0.0);
				v_blurTexCoords[ 9] = textureCoordinates.st + vec2( 0.012, 0.0);
				v_blurTexCoords[10] = textureCoordinates.st + vec2( 0.016, 0.0);
				v_blurTexCoords[11] = textureCoordinates.st + vec2( 0.020, 0.0);
				v_blurTexCoords[12] = textureCoordinates.st + vec2( 0.024, 0.0);
				v_blurTexCoords[13] = textureCoordinates.st + vec2( 0.028, 0.0);
			}
			#endif
 
			#ifdef FRAGMENT
			void main()
			{
				if(_Filter != 0)
				{
					gl_FragColor = vec4(0.0);
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 0].st)*0.0044299121055113265;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 1].st)*0.00895781211794;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 2].st)*0.0215963866053;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 3].st)*0.0443683338718;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 4].st)*0.0776744219933;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 5].st)*0.115876621105;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 6].st)*0.147308056121;
					gl_FragColor += texture2D(_DepthTex, textureCoordinates.st)*0.159576912161;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 7].st)*0.147308056121;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 8].st)*0.115876621105;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[ 9].st)*0.0776744219933;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[10].st)*0.0443683338718;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[11].st)*0.0215963866053;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[12].st)*0.00895781211794;
					gl_FragColor += texture2D(_DepthTex, v_blurTexCoords[13].st)*0.0044299121055113265;
				}
				else
				{
					gl_FragColor = texture2D(_DepthTex, vec2(textureCoordinates.s, textureCoordinates.t));
				}
			}
			
			#endif

			ENDGLSL
		}
	}
}

