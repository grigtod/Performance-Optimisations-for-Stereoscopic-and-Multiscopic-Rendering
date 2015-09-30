Shader "FoveatedInterlaced" 
{	
	Properties {
		_ViewTexLeft ("1st (RGB)", RECT) = "white" {}
		_ViewTexRight ("2nd (RGB)", RECT) = "white" {}
	
		_LDViewTexLeft ("1st LD (RGB)", RECT) = "white" {}
		_LDViewTexRight ("2nd LD (RGB)", RECT) = "white" {}
		
		_WidthRes ("Width", int) = 1920 
	    _HeightRes ("Height", int) = 1080 
	    
	    _multiplX ("Segment multiplier X", float) = 0
	    _multiplY  ("Segment multiplier Y", float) = 0
	    
	    _LDmultiplX ("Segment multiplier X", float) = 0
	    _LDmultiplY  ("Segment multiplier Y", float) = 0
	   
	    _focusPointX ("focusPointX", float) = 0
	    _focusPointY ("focusPointY", float) = 0
	    _LDRegionResolutionMultiplier ("LD region resolution multiplier", float) = 0
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

		uniform sampler2D _ViewTexLeft;
		uniform sampler2D _ViewTexRight;
		uniform sampler2D _LDViewTexLeft;
		uniform sampler2D _LDViewTexRight;
		
		float _WidthRes; 
		float _HeightRes;
		float _multiplX;
		float _multiplY;
		float _focusPointX;
		float _focusPointY;
		float _LDRegionResolutionMultiplier;
		
		fixed4 frag (v2f_img i) : SV_Target
		{	
			fixed4 output;
			float2 uv = float2(i.uv.x, i.uv.y);
			float X = (uint)(i.uv.x * _WidthRes);
			float Y = (uint)((i.uv.y) * _HeightRes);
			
			float mX = _focusPointX;
			float mY = _focusPointY; 
			
			float oX = (_HeightRes*_LDRegionResolutionMultiplier)/2;
			float2 t;
			
			
			if(X> mX-oX && Y > mY-oX && X < mX+oX && Y < mY+oX)  
			{
				t = float2( (X - (mX - oX)) /  (  (mX + oX) -  (mX - oX)  )  , (Y - (mY - oX)) /  (  (mY + oX) -  (mY - oX)  ) );
				if(X%2==0)
				{
					output = tex2D(_LDViewTexLeft,t);
				}
				else
				{
					output = tex2D(_LDViewTexRight,t);
				}
			}
			else
			{
				if(X%2==0)
				{
					output = tex2D(_ViewTexLeft,uv);
				}
				else
				{
					output = tex2D(_ViewTexRight,uv);
				}
			}
			
			return output;
			
		}
		ENDCG			
		}

	} 
	FallBack off
}