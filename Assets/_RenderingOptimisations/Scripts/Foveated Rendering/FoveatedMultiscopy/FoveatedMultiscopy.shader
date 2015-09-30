Shader "FoveatedMultiscopy" 
{	
	Properties {
		_ViewTex1 ("1st (RGB)", RECT) = "white" {}
		_ViewTex2 ("2nd (RGB)", RECT) = "white" {}
		_ViewTex3 ("3rd (RGB)", RECT) = "white" {}
		_ViewTex4 ("4th (RGB)", RECT) = "white" {}
		_ViewTex5 ("5th (RGB)", RECT) = "white" {}
		_ViewTex6 ("6th (RGB)", RECT) = "white" {}
		_ViewTex7 ("7nd (RGB)", RECT) = "white" {}
		_ViewTex8 ("8th (RGB)", RECT) = "white" {}
		
		_LDViewTex1 ("1st LD (RGB)", RECT) = "white" {}
		_LDViewTex2 ("2nd LD (RGB)", RECT) = "white" {}
		_LDViewTex3 ("3rd LD (RGB)", RECT) = "white" {}
		_LDViewTex4 ("4th LD (RGB)", RECT) = "white" {}
		_LDViewTex5 ("5th LD (RGB)", RECT) = "white" {}
		_LDViewTex6 ("6th LD (RGB)", RECT) = "white" {}
		_LDViewTex7 ("7nd LD (RGB)", RECT) = "white" {}
		_LDViewTex8 ("8th LD (RGB)", RECT) = "white" {}
		
		_WidthRes ("Width", int) = 1920 
	    _HeightRes ("Height", int) = 1080 
	    
	    _multiplX ("Segment multiplier X", float) = 0
	    _multiplY  ("Segment multiplier Y", float) = 0
	   
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

		uniform sampler2D _ViewTex1;
		uniform sampler2D _ViewTex2;
		uniform sampler2D _ViewTex3;
		uniform sampler2D _ViewTex4;
		uniform sampler2D _ViewTex5;
		uniform sampler2D _ViewTex6;
		uniform sampler2D _ViewTex7;
		uniform sampler2D _ViewTex8;
		uniform sampler2D _LDViewTex1;
		uniform sampler2D _LDViewTex2;
		uniform sampler2D _LDViewTex3;
		uniform sampler2D _LDViewTex4;
		uniform sampler2D _LDViewTex5;
		uniform sampler2D _LDViewTex6;
		uniform sampler2D _LDViewTex7;
		uniform sampler2D _LDViewTex8;
		float _WidthRes; 
		float _HeightRes;
		float _multiplX;
		float _multiplY;
		float _focusPointX;
		float _focusPointY;
		float _LDRegionResolutionMultiplier;
		//int _camOffset;
		float2 t;

		fixed4 getRGB (int viewIndex,int Xpixel,int Ypixel)
		{	
		
			fixed4 output;

			int X = (int)(Xpixel);
			int Y = (int)(Ypixel);
			float mX = _focusPointX/3;
			float mY = _focusPointY;

			float oX = (_HeightRes*_LDRegionResolutionMultiplier)/6;
			float oY = (_HeightRes*_LDRegionResolutionMultiplier)/2;

			if(X > mX-oX && Y > mY-oY && X < mX+oX &&  Y < mY+oY)  
			{
				t = float2( (X - (mX - oX)) /  ( (mX + oX) - (mX - oX) )  ,  
						    (Y - (mY - oY)) /  ( (mY + oY) - (mY - oY) )  );
				switch(viewIndex) //because the array can only be used with constant index (find solution later...)   ///14725836
				{
					case 0:  output = tex2D(_LDViewTex6, t);
					break;
					case 1:  output = tex2D(_LDViewTex3, t);
					break;
					case 2:  output = tex2D(_LDViewTex8, t);
					break;
					case 3:  output = tex2D(_LDViewTex5, t);
					break;
					case 4:  output = tex2D(_LDViewTex2, t);
					break;
					case 5:  output = tex2D(_LDViewTex7, t);
					break;
					case 6:  output = tex2D(_LDViewTex4, t);
					break;
					case 7:  output = tex2D(_LDViewTex1, t);
					break;
					case 8:  output = tex2D(_LDViewTex6, t);
					break; 
				}
			}
			else
			{
				float2 XY = float2(Xpixel * _multiplX,Ypixel *_multiplY);
				switch(viewIndex) //because the array can only be used with constant index (find solution later...)   ///14725836 
				{
					case 0:  output = tex2D(_ViewTex6,XY);
					break;
					case 1:  output = tex2D(_ViewTex3,XY);
					break;
					case 2:  output = tex2D(_ViewTex8,XY);
					break;
					case 3:  output = tex2D(_ViewTex5,XY);
					break;
					case 4:  output = tex2D(_ViewTex2,XY);
					break; 
					case 5:  output = tex2D(_ViewTex7,XY);
					break;
					case 6:  output = tex2D(_ViewTex4,XY);
					break;
					case 7:  output = tex2D(_ViewTex1,XY);
					break;
					case 8:  output = tex2D(_ViewTex6,XY);
					break; //8==1, I just put this here in case....
				}
			}
			return output;
		}
		
		fixed4 frag (v2f_img i) : SV_Target
		{	
			fixed4 output;
			uint X = (int)(i.uv.x * _WidthRes);
			uint Y = (int)((i.uv.y) * _HeightRes);
			
			uint base = (X+(Y/3))%8;
			switch(Y%3)
			{
				case 2: output = fixed4(getRGB((X+(Y/3))%8  ,(X/3),Y).x,getRGB((X+(Y/3)+3)%8,(X/3),Y).y,getRGB((X+(Y/3)+6)%8,(X/3),Y).z,1);
				break;
				case 1: output = fixed4(getRGB((X+(Y/3)-3)%8,(X/3),Y).x,getRGB((X+(Y/3))%8  ,(X/3),Y).y,getRGB((X+(Y/3)+3)%8,(X/3),Y).z,1);
				break;
				case 0: output = fixed4(getRGB((X+(Y/3)-6)%8,(X/3),Y).x,getRGB((X+(Y/3)-3)%8,(X/3),Y).y,getRGB((X+(Y/3))%8  ,(X/3),Y).z,1);
				break;	
				default: output = fixed4(0,0,0,1);
				break;
			}
			return output;
		}
		ENDCG			
		}

	} 
	FallBack off
}