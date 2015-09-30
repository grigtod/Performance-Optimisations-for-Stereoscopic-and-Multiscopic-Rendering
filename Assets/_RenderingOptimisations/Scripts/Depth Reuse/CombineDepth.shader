Shader "Hidden/CombineDepth" 
{ 
Properties 
{
	_FarTex ("Far Render Texture", 2D) = "white" {}
	_NearTex ("Near Render Texture", 2D) = "white" {}
}

SubShader 
{
	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform sampler2D _FarTex;
		uniform sampler2D _NearTex;
		fixed4 _HelperCol; 

		fixed4 frag (v2f_img i) : SV_Target
		{	
			fixed4 far = tex2D(_FarTex, float2(i.uv.x,1-i.uv.y) );		// the render texture is flipped in X and Y when displayed. This is a fix
			fixed4 near = tex2D(_NearTex, float2(i.uv.x,1-i.uv.y) );
			fixed4 output ;

			if(near.w<1)		// if the texel has any transparency output the far texture.
			{
				output = far;
			}
			else
			{
				output = near;
			}
			return output;
		}
		ENDCG
	}
}

Fallback off

}
