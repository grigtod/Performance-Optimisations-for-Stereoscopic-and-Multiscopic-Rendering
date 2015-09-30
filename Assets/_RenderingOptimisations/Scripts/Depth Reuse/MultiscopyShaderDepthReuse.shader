Shader "MultiscopyShaderWithDepthReuse" 
{	
	Properties {
		_FarDepthTex ("FarDepth", RECT) = "white" {}  // additional texture  for the depth render texture
		_ViewTex1 ("1st (RGB)", RECT) = "white" {}
		_ViewTex2 ("2nd (RGB)", RECT) = "white" {}
		_ViewTex3 ("3rd (RGB)", RECT) = "white" {}
		_ViewTex4 ("4th (RGB)", RECT) = "white" {}
		_ViewTex5 ("5th (RGB)", RECT) = "white" {}
		_ViewTex6 ("6th (RGB)", RECT) = "white" {}
		_ViewTex7 ("7nd (RGB)", RECT) = "white" {}
		_ViewTex8 ("8th (RGB)", RECT) = "white" {}
		
		_WidthRes ("Width", int) = 1920 
	    _HeightRes ("Height", int) = 1080 
	    
	    _multiplX ("Segment multiplier X", float) = 0 // this is used to determine the pixel location of each texel
	    _multiplY  ("Segment multiplier Y", float) = 0
	    
	    _camOffset ("Camera offset", int)=0
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

		uniform sampler2D _FarDepthTex;
		uniform sampler2D _ViewTex1;
		uniform sampler2D _ViewTex2;
		uniform sampler2D _ViewTex3;
		uniform sampler2D _ViewTex4;
		uniform sampler2D _ViewTex5;
		uniform sampler2D _ViewTex6;
		uniform sampler2D _ViewTex7;
		uniform sampler2D _ViewTex8;
		
		float _WidthRes; 
		float _HeightRes;
		float _multiplX;
		float _multiplY;
		uint _camOffset;
		
		int tempTest;
		fixed4 getRGB (int viewIndex,int Xpixel,int Ypixel)
		{		
			fixed4 output;
			viewIndex=(viewIndex+_camOffset)%8;
			switch(viewIndex) ///14725836 - the order of the views that are being displaye
			{
				case 0:  output = tex2D(_ViewTex6,float2(Xpixel * _multiplX,Ypixel *_multiplY));tempTest = 6;
				break;
				case 1:  output = tex2D(_ViewTex3,float2(Xpixel * _multiplX,Ypixel *_multiplY));tempTest = 3;
				break;
				case 2:  output = tex2D(_ViewTex8,float2(Xpixel * _multiplX,Ypixel *_multiplY));tempTest = 8;
				break;
				case 3:  output = tex2D(_ViewTex5,float2(Xpixel * _multiplX,Ypixel *_multiplY));tempTest = 5;
				break;
				case 4:  output = tex2D(_ViewTex2,float2(Xpixel * _multiplX,Ypixel *_multiplY));tempTest = 2;
				break;
				case 5:  output = tex2D(_ViewTex7,float2(Xpixel * _multiplX,Ypixel *_multiplY));tempTest = 7;
				break;
				case 6:  output = tex2D(_ViewTex4,float2(Xpixel * _multiplX,Ypixel *_multiplY));tempTest = 4;
				break;
				case 7:  output = tex2D(_ViewTex1,float2(Xpixel * _multiplX,Ypixel *_multiplY));tempTest = 1;
				break;
				case 8:  output = tex2D(_ViewTex6,float2(Xpixel * _multiplX,Ypixel *_multiplY));tempTest = 6;
				break; //8==1, I just put this here in case....
			}
			
			if(output.w <1)
			{
				output = tex2D(_FarDepthTex, float2(Xpixel * _multiplX - tempTest*_multiplX * 2, Ypixel *_multiplY));
			}

			return output;
		}
		
		fixed4 frag (v2f_img i) : SV_Target
		{	
			fixed4 output;
			uint X = (int)((i.uv.x) * _WidthRes);
			uint Y = (int)((1-i.uv.y) * _HeightRes);
								
			uint base = (X+(Y/3))%8;
			switch(Y%3)		//depending on the pixel location the views of the rgb values are chosen. 
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