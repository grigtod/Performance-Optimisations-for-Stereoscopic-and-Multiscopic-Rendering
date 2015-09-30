Shader "Hidden/FoveatedRegionShader" {
Properties 
{
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_MaskTex("Base (RGB)", 2D) = "mask" {}
}

SubShader 
{
	Pass 
	{
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform sampler2D _MaskTex;

		fixed4 frag (v2f_img i) : SV_Target
		{
			fixed4 original = tex2D(_MainTex, i.uv);
			fixed4 mask = tex2D(_MaskTex, i.uv);
			fixed4 output = original;
			output.a = mask.a;
			return output;
		}
		ENDCG

	}
}

Fallback off

}
