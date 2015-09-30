using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    public class FoveatedRegionImageEffect : ImageEffectBaseCustom 
	{
        public Texture  texture;

        void OnRenderImage (RenderTexture source, RenderTexture destination) 
		{
			material.SetTexture("_MaskTex", texture);
            Graphics.Blit (source, destination, material);
        }
    }
}
