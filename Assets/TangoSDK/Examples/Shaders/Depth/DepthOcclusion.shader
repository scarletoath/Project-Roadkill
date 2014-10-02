Shader "Tango/PostProcess/DepthOcclusion" {
	Properties 
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_ColorTex ("Color Texture", 2D) = "white" {}	
		_DepthTex ("Depth Texture", 2D) = "white" {}
		_Filter("Filter", int) = 0
		_IsShowingDepth("IsShowingDepth", int) = 0
		_IsEnabledOcclusion("IsEnabledOcclusion", int) = 1
	}
	SubShader 
	{
		Pass 
		{
			GLSLPROGRAM
			#include "UnityCG.glslinc"
			uniform sampler2D _MainTex;
			uniform sampler2D _ColorTex;
			uniform sampler2D _DepthTex;	
			
     		uniform int _Filter;
     		uniform int _IsShowingDepth;
     		uniform int _IsEnabledOcclusion;
     		
     		uniform sampler2D _CameraDepthTexture;
     		   		
     		uniform vec4 _ZBufferParams;
         	varying vec4 textureCoordinates; 
			
			#ifdef VERTEX
			void main()
			{
				textureCoordinates = gl_MultiTexCoord0;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
			#endif
 
			#ifdef FRAGMENT
			float Linear01Depth(float z ) 
			{ 
				return 1.0 / (_ZBufferParams.x * z + _ZBufferParams.y); 
			}
						
			void main()
			{			
				// depth from device									
				float realDepth = texture2D(_DepthTex, vec2(textureCoordinates.s, textureCoordinates.t)).r;
				// depth from virtual camera in the scene
				float virtualDepth = Linear01Depth(texture2D(_CameraDepthTexture, vec2(textureCoordinates.s, textureCoordinates.t)).r);

				float alpha = 1.0;	
				
//				gl_FragColor = vec4(virtualDepth, 0.0, 0.0, 1.0);
				if(_IsEnabledOcclusion == 1)
				{	
					if(realDepth < virtualDepth )
					{
						// not show ufo
						if(_IsShowingDepth == 1)
						{
							vec4 t0 = texture2D(_ColorTex, vec2(textureCoordinates.s, 1.0-textureCoordinates.t));
				  			vec4 t1 = texture2D(_DepthTex, textureCoordinates.st);
				  			gl_FragColor = (1.0-alpha) * t0 + alpha * t1;
						}
						else
						{
							gl_FragColor = texture2D(_ColorTex, vec2(textureCoordinates.s, 1.0-textureCoordinates.t));
						}
					}
				    else
		            {
		            	if(_IsShowingDepth == 1)
						{
							vec4 t0 = texture2D(_MainTex, vec2(textureCoordinates.s, textureCoordinates.t));
				  			vec4 t1 = texture2D(_DepthTex, textureCoordinates.st);
				  			gl_FragColor = (1.0-alpha) * t0 + alpha * t1;
						}
						else
						{
							gl_FragColor = texture2D(_MainTex, vec2(textureCoordinates.s, textureCoordinates.t));
						}
		            }
		         }
		         else
		         {
		         	if(texture2D(_MainTex, vec2(textureCoordinates.s, textureCoordinates.t)).a > 0.1)
		         	{	
		         		gl_FragColor = texture2D(_MainTex, vec2(textureCoordinates.s, textureCoordinates.t));
		         	}
		         	else
		         	{
		         		gl_FragColor = texture2D(_ColorTex, vec2(textureCoordinates.s, 1.0-textureCoordinates.t));
		         	}
		         }
			}
			
			#endif

			ENDGLSL
		}
	}
}
