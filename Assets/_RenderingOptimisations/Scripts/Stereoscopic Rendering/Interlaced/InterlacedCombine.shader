Shader "InterlacedCombine" 
{	
	Properties {
		_LeftTex ("Left Eye Rend Tex", RECT) = "white" {}
		_RightTex ("Right Eye Rend Tex", RECT) = "white" {}
		_WidthRes ("Width", int) = 1920 
	}
	
	SubShader {
		
		Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#include "UnityCG.cginc"

		uniform sampler2D _LeftTex;
		uniform sampler2D _RightTex;
		float _WidthRes; 
		
		fixed4 frag (v2f_img i) : SV_Target
		{	
			fixed4 output;
			float2 uv = float2(i.uv.x, 1-i.uv.y);	// the y needs to be flipped
			uint X = (int)(uv.x * _WidthRes);	// determine the pixel location of the texel
			if(X%2==0)		// choses the appropreate view 
			{
				output = tex2D(_LeftTex,uv);
			}
			else
			{
				output = tex2D(_RightTex,uv);
			}
			return output;
		}
		ENDCG			
		}
	} 
	FallBack off
}