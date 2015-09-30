Shader "SideBySide" 
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
			float2 uv = float2(i.uv.x, 1-i.uv.y);
			int X = (int)(uv.x * _WidthRes);
			if(uv.x<=0.5)
			{
				output = tex2D(_LeftTex,float2(uv.x*2, uv.y));
			}
			else
			{
				output = tex2D(_RightTex,float2(uv.x*2-1, uv.y));
			}
			return output;
		}
		ENDCG			
		}
	} 
	FallBack off
}