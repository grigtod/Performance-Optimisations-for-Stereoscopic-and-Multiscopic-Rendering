Shader "InterlacedDepthReuse" 
{	
	Properties {
		_LeftTex ("Left Eye Rend Tex", RECT) = "white" {}
		_RightTex ("Right Eye Rend Tex", RECT) = "white" {}
		_FarDepthTex ("Depth Rend Tex", RECT) = "white" {}
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
		uniform sampler2D _FarDepthTex;
		float _WidthRes; 
		
		fixed4 frag (v2f_img i) : SV_Target
		{	
			fixed4 output;
			float2 uv = float2(i.uv.x, 1-i.uv.y);
			uint X = (int)(uv.x * _WidthRes);
			if(X%2==0)
			{
				output = tex2D(_LeftTex,uv);
			}
			else
			{
				output = tex2D(_RightTex,uv);
			}
			
			if(output.w <1 )	// if the output is transparent the depth texture is used
			{
				output = tex2D(_FarDepthTex, uv);
			}
			
			return output;
		}
		ENDCG			
		}
	} 
	FallBack off
}