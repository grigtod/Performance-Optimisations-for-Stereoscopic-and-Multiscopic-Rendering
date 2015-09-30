using System;
using UnityEngine;


namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]     //in order to test the result of this image effect in the editor even if the application is not playing this line enables it's execution in editor.
    [RequireComponent(typeof(Camera))]  //camera component is required 
    public class CombineDepth : ImageEffectBaseCustom         //this script inherits from the "ImageEffectBaseCustom.cs" script which is a modified script from the Unity Image Effects. 
    {
        [Tooltip("Reference to the far camera that renders the distant game objects.")]
        public Camera farCam;               
        [Tooltip("Reference to the near camera that renders the near game objects.")]
        public Camera nearCam;              
        [Tooltip("This variable is multiplied with the screen reslution to determine the resolution of the render texture for the 'farCam'.")]
        public float farRenderTextureResolutionMultiplier = 0.1f;

        private RenderTexture farRT;        //the render texture of the far camera
        private RenderTexture nearRT;
         
        void Start()
        {
            base.StartFirst(); // this is the main reason why the original ImageEffectBase.cs script from Unity was modified. 
            farRenderTextureResolutionMultiplier = Mathf.Clamp(farRenderTextureResolutionMultiplier, 0.01f, 10);
            try
            {
                farRT = new RenderTexture((int)(Screen.width * farRenderTextureResolutionMultiplier), (int)(Screen.height * farRenderTextureResolutionMultiplier), 16, RenderTextureFormat.ARGB32);     //create the render texture of the camera that renders distance objects
                nearRT = new RenderTexture((int)(Screen.width), (int)(Screen.height), 16, RenderTextureFormat.ARGB32);      //create the render texture of the camera that renders objects that are close 
                farCam.targetTexture = farRT;                   //assign the render textures to the cameras
                nearCam.targetTexture = nearRT;
                material.SetTexture("_FarTex", farRT);          //set the shader far and near textures.
                material.SetTexture("_NearTex", nearRT);
            }
            catch
            {
                Debug.LogError("Missing references in " + this.name + ".cs . Please provide a reference.");
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)        //this function is called every time Unity attempts to render the screen
        {
            Graphics.Blit(source, destination, material);      //draws a quad with the output from the shader
        }
    }

}

